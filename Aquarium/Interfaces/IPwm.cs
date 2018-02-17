namespace Aquarium
{
    public interface IPwm
    {
        bool SetPwm(int pin, int value);
    }
}