#include <UnityRobot.h>
#include <DigitalModule.h>

DigitalModule digital(0, 47);

void OnUpdate(byte id)
{
  digital.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  digital.action();
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  digital.reset();
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
}

void OnReady(void)
{
  digital.flush();
}

void setup()
{
  digital.begin();
  
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
