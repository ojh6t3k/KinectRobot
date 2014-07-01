#include <UnityRobot.h>
#include <ServoModule.h>
#include <Servo.h>

ServoModule servoModule(0, 47, -90, 90);
Servo servo;

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
    servo.write(servoModule.getValue(90, 1));
  }
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  servoModule.reset();
  servo.write(90);
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
  servo.attach(servoModule.getPin());
  
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
