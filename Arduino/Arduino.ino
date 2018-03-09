/*
Name:    Sketch1.ino
Created: 9/13/2017 7:25:36 PM
Author:  Pavel Mazalek
*/

#include <string.h>
#include <OneWire.h>
#include <DallasTemperature.h>
const String _version = "0.2";
const String _programName = "Aquarium Controller";
const String _endConstant = "END#";

#define USONIC_DIV 58.0

// the setup function runs once when you press reset or power the board
void setup()
{
	Serial.begin(115200);
}

// the loop function runs over and over again until power down or reset
void loop()
{


	if (Serial.available() > 0)
	{
		String incommingMessage = Serial.readString();

		String command = GetValueFromString(incommingMessage, 0);

		if (command == "info" or command == "info\r" or command == "info\r\n")
		{

			WriteOnSerial("OK " + GetInfo() + _endConstant);
			return;
		}

		if (command == "setpwm" or command == "setpwm\r" or command == "setpwm\r\n")
		{
			String pin = GetValueFromString(incommingMessage, command.length());
			String pwmValue = GetValueFromString(incommingMessage, (command.length() + pin.length() + 1));

			SetPWM(pin.toInt(), pwmValue.toInt());
			WriteOnSerial("OK" + _endConstant);

			return;
		}

		if (command == "gettemps")
		{
			String pin = GetValueFromString(incommingMessage, command.length());
			OneWire oneWireDS(pin.toInt());
			DallasTemperature senzoryDS(&oneWireDS);
			senzoryDS.begin();
			senzoryDS.requestTemperatures();

			int count = senzoryDS.getDeviceCount();

			String result = "";
			int i = 0;
			for (i = 0; i< count; i++)
			{
				DeviceAddress address;
				senzoryDS.getAddress(address, i);

				result += AddressToString(address) + "|" + senzoryDS.getTempCByIndex(i) + ";";
			}
			Serial.println(result + _endConstant);

			return;
		}

		if (command == "getlight")
		{
			String pin = GetValueFromString(incommingMessage, command.length());
			Serial.println(analogRead(pin.toInt()) + _endConstant);

			return;
		}

		if (command == "getdistance")
		{
			String triggerPin = GetValueFromString(incommingMessage, command.length());
			String echoPin = GetValueFromString(incommingMessage, command.length() + triggerPin.length() + 1);
			String samples = GetValueFromString(incommingMessage, command.length() + triggerPin.length() + echoPin.length() + 2);

			pinMode(triggerPin.toInt(), OUTPUT);
			pinMode(echoPin.toInt(), INPUT);
			int distance = measure(samples.toInt(), triggerPin.toInt(), echoPin.toInt());
			Serial.println(distance + _endConstant);
			return;
		}



		WriteOnSerial("FALSE " + command);
	}
}

long measure(int samples, int trigger, int echo)
{
	long measureSum = 0;
	for (int i = 0; i < samples; i++)
	{
		delay(5);
		measureSum += singleMeasurement(trigger, echo);
	}
	return measureSum / samples;
}

long singleMeasurement(int trigger, int echo)
{
	long duration = 0;
	// Measure: Put up Trigger...
	digitalWrite(trigger, HIGH);
	// ... wait for 11 �s ...
	delayMicroseconds(11);
	// ... put the trigger down ...
	digitalWrite(trigger, LOW);
	// ... and wait for the echo ...
	duration = pulseIn(echo, HIGH);
	return (long)(((float)duration / USONIC_DIV) * 10.0);
}


void SetPWM(int pin, int value)
{
	analogWrite(pin, value);
}

void WriteOnSerial(String message)
{
	delay(50);
	Serial.print(message);
}

String GetInfo()
{
	return _programName + "  " + _version;
}

String AddressToString(DeviceAddress deviceAddress)
{
	String address = "";
	for (uint8_t i = 0; i < 8; i++)
	{
		if (deviceAddress[i] < 16) Serial.print("0");
		address += String(deviceAddress[i], HEX);
	}

	return address;
}

String GetValueFromString(String message, int startIndex)
{
	if (startIndex > 0)
		startIndex++;

	int endIndex = message.indexOf(';', startIndex);
	String value = message.substring(startIndex, endIndex);
	value.toLowerCase();

	//Serial.println("------Debg-----");
	//Serial.print(startIndex);
	//Serial.print(" ");
	//Serial.print(endIndex);
	//Serial.print(" ");
	//Serial.println(value);
	//Serial.println("------END------");

	return value;
}

