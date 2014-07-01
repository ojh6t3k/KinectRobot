#include <UnityRobot.h>
#include <PWMModule.h>

PWMModule pwm(0, 16);

void OnUpdate(byte id)
{
  pwm.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  pwm.action();
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  pwm.reset();  
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
  pwm.begin();
  
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
