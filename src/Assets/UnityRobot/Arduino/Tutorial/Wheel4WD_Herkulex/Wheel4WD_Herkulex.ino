#include <UnityRobot.h>
#include <WheelController.h>
#include <Herkulex.h>

// Herkulex ID
#define WHEEL_FL 1
#define WHEEL_BL 2
#define WHEEL_BR 3
#define WHEEL_FR 4

WheelController driver(0);


void OnUpdate(byte id)
{
  driver.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  if(driver.updated() == true)
  {
    Herkulex.moveSpeedOne(WHEEL_FL, (int)driver.lSpeed(2048, 1), 500, LED_PINK);
    Herkulex.moveSpeedOne(WHEEL_BL, (int)driver.lSpeed(2048, 1), 500, LED_PINK);
    Herkulex.moveSpeedOne(WHEEL_BR, (int)driver.rSpeed(2048, 1), 500, LED_PINK);
    Herkulex.moveSpeedOne(WHEEL_FR, (int)driver.rSpeed(2048, 1), 500, LED_PINK);
  }
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  driver.reset();
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
  Herkulex.moveSpeedOne(WHEEL_FL, 0, 500, LED_PINK);
  Herkulex.moveSpeedOne(WHEEL_BL, 0, 500, LED_PINK);
  Herkulex.moveSpeedOne(WHEEL_BR, 0, 500, LED_PINK);
  Herkulex.moveSpeedOne(WHEEL_FR, 0, 500, LED_PINK);
}

void OnReady(void)
{
}

void setup()
{
  Herkulex.beginSerial1(115200); //open serial
  Herkulex.reboot(WHEEL_FL);
  Herkulex.reboot(WHEEL_BL);
  Herkulex.reboot(WHEEL_BR);
  Herkulex.reboot(WHEEL_FR);
  delay(500);
  Herkulex.initialize(); //initialize motors
  delay(200);
  
  UnityRobot.attach(CMD_UPDATE, OnUpdate);
  UnityRobot.attach(CMD_ACTION, OnAction);
  UnityRobot.attach(CMD_START, OnStart);
  UnityRobot.attach(CMD_EXIT, OnExit);
  UnityRobot.attach(CMD_READY, OnReady);
  UnityRobot.begin(57600);
}

void loop()
{
  UnityRobot.process();
}
