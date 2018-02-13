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
const int _serialBaudRate = 115200;

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
		String incomingMessage = Serial.readString();

		String command = GetValueFromString(incomingMessage, 0);

		if (command == "info" or command == "info\r" or command == "info\r\n")
		{

			WriteOnSerial("OK " + GetInfo());
			return;
		}

		if (command == "setpwm" or command == "setpwm\r" or command == "setpwm\r\n")
		{
			String pin = GetValueFromString(incomingMessage, command.length());
			String pwmValue = GetValueFromString(incomingMessage, (command.length() + pin.length() + 1));

			SetPWM(pin.toInt(), pwmValue.toInt());
			WriteOnSerial("OK");

			return;
		}

    
		if (command == "gettemp")
		{    
			String pin = GetValueFromString(incomingMessage, command.length());
			String index = GetValueFromString(incomingMessage, command.length() + pin.length());
			OneWire oneWireDS(pin.toInt());
			DallasTemperature senzoryDS(&oneWireDS);
			senzoryDS.requestTemperatures();
			Serial.println(senzoryDS.getTempCByIndex(index.toInt()));

      return;
		}

		WriteOnSerial("FALSE " + command);
	}
}

void SetPWM(int pin, int value)
{
	analogWrite(pin, value);
}

void WriteOnSerial(String message)
{
	delay(50);
	Serial.println(message);
}

String GetInfo()
{
	return _programName + "  " + _version;
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

