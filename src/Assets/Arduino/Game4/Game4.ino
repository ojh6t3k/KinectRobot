#include <UnityRobot.h>
#include <ADCModule.h>


ADCModule forward(0, A0);
ADCModule right(1, A1);
ADCModule left(2, A2);

void OnUpdate(byte id)
{
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
}

void OnReady(void)
{
  forward.flush();
  right.flush();
  left.flush(); 
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
