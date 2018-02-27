using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Aquarium.Log;
using Aquarium.Settings;

namespace Aquarium
{
    public class LightManager : ILightManager
    {
        private ConfigLIghts configLights;
        private Timer timer;
        private int timerInterval = 2000;
        private IArduinoComunication arduino;
        private int currentIntensity = 0;
        private IEnumerable<LightState> lightStates;
        private ILogger logger;
        private IConfigManager configManager;
        private DayOfWeek dayOfWeek;

        public LightManager(IArduinoComunication arduino, ILogger logger, IConfigManager configManager)
        {
            this.arduino = arduino;
            this.logger = logger;
            this.configManager = configManager;

            
            timer = new Timer(timerInterval);
            dayOfWeek = DateTime.Now.DayOfWeek;
        }

        private void ConfigManager_ConfigChanged(object sender, EventArgs e)
        {
            BuildLightTable();
        }

        public void Run()
        {
            try
            {
                this.configManager.ConfigChanged += ConfigManager_ConfigChanged;

                if (!arduino.IsConnected)
                {
                    logger.Write("Cannot find Arduino. Finish program");
                    return;
                }

                logger.Write("Starting light controller", LoggerTypes.LogLevel.Info);
                GetConfig();
                timerInterval = (configLights.LightMaxValue - configLights.LightMinValue) /
                                configLights.FullTurningOnMinutes * 60;

                logger.Write($"Interval for light change is {timerInterval} seconds.", LoggerTypes.LogLevel.Info);

                BuildLightTable();
                SetCurrentState();

                timer.Interval = timerInterval;
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
        }

        private void SetCurrentState()
        {
            var lightState = this.lightStates.Where(p => p.Status == LightDayStatus.Skip).OrderBy(p => p.DayOfWeek).ThenBy(p => p.Time).Last();
            logger.Write($"Setting startup intensity {lightState.DayOfWeek}\t{lightState.Time}\t{lightState.LightIntensity}", LoggerTypes.LogLevel.System);
            arduino.SetPWM(configLights.LightPinNumber, PercentToValue(lightState.LightIntensity));
            currentIntensity = PercentToValue(lightState.LightIntensity);
        }

        private IOrderedEnumerable<LightState> GetSortedTimes()
        {
            return this.lightStates
                .OrderBy(p => p.DayOfWeek)
                .ThenBy(p => p.Time);
        }

        private int PercentToValue(int percent)
        {
            var range = configLights.LightMaxValue - configLights.LightMinValue;
            var constant = range / 100;

            var value = percent * constant;
            return value;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                CheckDayChange();
                dayOfWeek = DateTime.Now.DayOfWeek;

                var nextPlanned =
                    this.lightStates.Where(p => p.Status == LightDayStatus.Planned)
                        .OrderBy(p => p.DayOfWeek)
                        .ThenBy(p => p.Time)
                        .FirstOrDefault();

                if (nextPlanned == null)
                    return;

                logger.Write(
                    $"Nex planned on time {nextPlanned.Time} with intensity {nextPlanned.LightIntensity}. Current intensity {currentIntensity}",
                    LoggerTypes.LogLevel.Info);

                if (nextPlanned.DayOfWeek == DateTime.Now.DayOfWeek &&
                    nextPlanned.Time < DateTime.Now.TimeOfDay)
                {
                    if (currentIntensity < PercentToValue(nextPlanned.LightIntensity))
                        currentIntensity++;

                    if (currentIntensity > PercentToValue(nextPlanned.LightIntensity))
                        currentIntensity--;

                    if (currentIntensity == PercentToValue(nextPlanned.LightIntensity))
                    {
                        nextPlanned.Status = LightDayStatus.Passed;
                        logger.Write($"Change on {nextPlanned.DayOfWeek}-{nextPlanned.Time} with intensity {nextPlanned.LightIntensity} is done.", LoggerTypes.LogLevel.System);

                        var tempNextChange = this.lightStates.Where(p => p.Status == LightDayStatus.Planned)
                        .OrderBy(p => p.DayOfWeek)
                        .ThenBy(p => p.Time)
                        .FirstOrDefault();

                        if (tempNextChange != null)
                            logger.Write($"Next change is planned on time {tempNextChange.DayOfWeek}-{tempNextChange.Time} with intensity {tempNextChange.LightIntensity}");
                    }

                    if (arduino == null)
                        return;

                    SetIntensity(configLights.LightPinNumber, currentIntensity);
                }
            }
            catch (Exception ex)
            {
                logger.Write(ex);
            }
        }

        public void SetIntensity(int pin, int intensity)
        {
            currentIntensity = intensity;
            arduino.SetPWM(pin, intensity);
        }

        public IEnumerable<LightState> GetLightTable()
        {
            return lightStates;
        }

        private void CheckDayChange()
        {
            if(dayOfWeek == DayOfWeek.Tuesday && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                BuildLightTable();
        }

        public void Stop()
        {
            this.configManager.ConfigChanged -= ConfigManager_ConfigChanged;
            timer.Stop();
        }

        private void GetConfig()
        {
            configLights = configManager.GetConfig().LightConfig;
        }

        private void BuildLightTable()
        {
            var lightStates = configLights.LightStates.ToList();

            if (lightStates.GroupBy(p => p.DayOfWeek).Count() != 7)
            {
                lightStates.RemoveAll(p => p.DayOfWeek != 0);

                var tempStates = new List<LightState>();

                foreach (var lightState in lightStates)
                {
                    tempStates.Add(lightState.Copy());
                }

                foreach (var sundayLightState in tempStates.Where(p => p.DayOfWeek == DayOfWeek.Sunday))
                {
                    lightStates.AddRange(CopyStateToAllDays(sundayLightState));
                }
            }

            logger.Write("----Light Table:", LoggerTypes.LogLevel.System);

            foreach (var lightState in lightStates.OrderBy(p => p.DayOfWeek).ThenBy(p => p.Time))
            {
                if (lightState.DayOfWeek < DateTime.Now.DayOfWeek)
                    lightState.Status = LightDayStatus.Skip;

                if (lightState.DayOfWeek > DateTime.Now.DayOfWeek)
                    lightState.Status = LightDayStatus.Planned;

                if (lightState.DayOfWeek == DateTime.Now.DayOfWeek)
                {
                    if (lightState.Time < DateTime.Now.TimeOfDay)
                        lightState.Status = LightDayStatus.Skip;
                    else
                        lightState.Status = LightDayStatus.Planned;
                }

                logger.Write($"{lightState.DayOfWeek.ToString().PadRight(10)}\t{lightState.Time}\t{lightState.LightIntensity}\t{PercentToValue(lightState.LightIntensity)}\t{lightState.Status}", LoggerTypes.LogLevel.System);
            }

            logger.Write("----End of Light Table", LoggerTypes.LogLevel.System);

            this.lightStates = lightStates;
        }

        private IEnumerable<LightState> CopyStateToAllDays(LightState sundayLightState)
        {
            var lightStates = new List<LightState>();

            var monday = sundayLightState.Copy();
            monday.DayOfWeek = DayOfWeek.Monday;

            var tuesday = sundayLightState.Copy();
            tuesday.DayOfWeek = DayOfWeek.Tuesday;

            var wednesday = sundayLightState.Copy();
            wednesday.DayOfWeek = DayOfWeek.Wednesday;

            var thursday = sundayLightState.Copy();
            thursday.DayOfWeek = DayOfWeek.Thursday;

            var friday = sundayLightState.Copy();
            friday.DayOfWeek = DayOfWeek.Friday;

            var saturday = sundayLightState.Copy();
            saturday.DayOfWeek = DayOfWeek.Saturday;

            lightStates.Add(monday);
            lightStates.Add(tuesday);
            lightStates.Add(wednesday);
            lightStates.Add(thursday);
            lightStates.Add(friday);
            lightStates.Add(saturday);

            return lightStates;
        }
    }
}
