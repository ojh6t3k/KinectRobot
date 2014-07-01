#include <UnityRobot.h>
#include <PanTiltController.h>
#include <Herkulex.h>

// Herkulex ID
#define PAN 8
#define TILT 9

PanTiltController panTilt(0);


void OnUpdate(byte id)
{
  panTilt.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  if(panTilt.updated() == true)
  {
    Herkulex.moveOneAngle(PAN, (int)panTilt.panAngle(180, 1), 0, LED_PINK);
    Herkulex.moveOneAngle(TILT, (int)panTilt.tiltAngle(180, -1), 0, LED_PINK);
  }
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  panTilt.reset();
  Herkulex.moveOneAngle(PAN, 0, 0, LED_PINK);
  Herkulex.moveOneAngle(TILT, 0, 0, LED_PINK);
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
  Herkulex.reboot(PAN);
  Herkulex.reboot(TILT);
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
