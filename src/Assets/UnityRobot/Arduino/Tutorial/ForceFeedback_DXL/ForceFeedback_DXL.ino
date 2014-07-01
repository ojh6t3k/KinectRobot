#include <UnityRobot.h>
#include <ForceFeedback.h>
#include <Dynamixel.h>

#define DIR_PIN  32
#define DIR_TX  HIGH
#define DIR_RX  LOW

#define DXL_ID   1
#define DXL_BAUD 1000000

ForceFeedback force(0);


void OnUpdate(byte id)
{
  force.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  if(force.updated() == true)
  {
    Dynamixel.write(DXL_ID, 34, (word)force.getForceLimit(10.24));
  }
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  force.reset();
  Dynamixel.joint(DXL_ID, 512);
  Dynamixel.write(DXL_ID, 34, (word)force.getForceLimit(10.24));
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
}

void OnReady(void)
{  
  word presentLoad;
  if(Dynamixel.read(DXL_ID, 40, &presentLoad) == COMMERROR_NONE)
  {
    float feedback = (float)(presentLoad & 0x3FF);
    if(presentLoad & 0x400)
      feedback *= -1;
    feedback *= 0.097;
    force.flush((short)feedback);
  }
  else
    force.flush(-100);
}

void setup()
{
  Dynamixel.attachSerial(&Serial1);
  Dynamixel.attachPins(DIR_PIN, DIR_TX, DIR_RX);
  Dynamixel.begin(DXL_BAUD);
  
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
