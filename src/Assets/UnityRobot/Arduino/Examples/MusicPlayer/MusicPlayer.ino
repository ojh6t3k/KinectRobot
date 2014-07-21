#include <UnityRobot.h>
#include <ToneModule.h>


ToneModule toneModule(0, 7);

void OnUpdate(byte id)
{
  toneModule.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  toneModule.action();
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  toneModule.reset();
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
  toneModule.reset();
}

void OnReady(void)
{
}

void setup()
{
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
