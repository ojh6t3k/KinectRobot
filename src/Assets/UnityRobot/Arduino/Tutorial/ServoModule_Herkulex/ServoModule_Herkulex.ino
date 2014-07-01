#include <UnityRobot.h>
#include <ServoModule.h>
#include <Herkulex.h>

#define HERKULEX_ID  6

ServoModule servoModule(0, HERKULEX_ID, -160, 160);

void OnUpdate(byte id)
{
  servoModule.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  if(servoModule.updated() == true)
  {
    Herkulex.moveOneAngle(HERKULEX_ID, (int)servoModule.getValue(0, 1), 0, LED_PINK);
  }
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  servoModule.reset();
  Herkulex.moveOneAngle(HERKULEX_ID, 0, 0, LED_PINK);
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
}

void OnReady(void)
{
}

void setup()
{
  Herkulex.beginSerial1(115200); //open serial
  Herkulex.reboot(HERKULEX_ID);
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
