#include <UnityRobot.h>
#include <DigitalModule.h>
#include <ADCModule.h>

DigitalModule button(0, 47);
ADCModule dial(1, A0);

void OnUpdate(byte id)
{
  button.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  button.action();
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  button.reset();
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
}

void OnReady(void)
{
  button.flush();
  dial.flush();  
}

void setup()
{
  button.begin();
  
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
