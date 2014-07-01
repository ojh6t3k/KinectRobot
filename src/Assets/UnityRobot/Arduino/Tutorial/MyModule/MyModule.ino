#include <UnityRobot.h>

class MyModule
{
  int _id;
  
  byte input_byte;
  word input_word;
  short input_short;
  
  byte output_byte;
  word output_word;
  short output_short;
  
public:
  MyModule(byte id);
  
  void Update(byte id);
  void Action();
  void Reset();
  void Flush();
};

MyModule module(0);

void OnUpdate(byte id)
{
  module.Update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  module.Action();
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  module.Reset();
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
}

void OnReady(void)
{
  module.Flush();
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
