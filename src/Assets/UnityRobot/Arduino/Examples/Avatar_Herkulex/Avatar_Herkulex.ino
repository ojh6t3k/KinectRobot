#include <UnityRobot.h>
#include <ServoModule.h>
#include <Herkulex.h>

ServoModule L_Shoulder1(0, 0, -160, 160); // L Shoulder
ServoModule L_Shoulder2(1, 1, -160, 160); // L Shoulder2
ServoModule L_Shoulder3(2, 2, -160, 160); // L Shoulder3
ServoModule L_Elbow(3, 3, -160, 160); // L Elbow
ServoModule R_Shoulder1(4, 4, -160, 160); // R Shoulder1
ServoModule R_Shoulder2(5, 5, -160, 160); // R Shoulder2
ServoModule R_Shoulder3(6, 6, -160, 160); // R Shoulder3
ServoModule R_Elbow(7, 7, -160, 160); // R Elbow

unsigned long preTime = 0;

void OnUpdate(byte id)
{
  L_Shoulder1.update(id);
  L_Shoulder2.update(id);
  L_Shoulder3.update(id);
  L_Elbow.update(id);
  R_Shoulder1.update(id);
  R_Shoulder2.update(id);
  R_Shoulder3.update(id);
  R_Elbow.update(id);
}

// When recieved end of update
void OnAction(void)
{
  boolean updated = false;
  //TODO: Synchronizing module's action
  if(L_Shoulder1.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(L_Shoulder1.getPin(), L_Shoulder1.getValue(512, 3.2), 0);
  }
  if(L_Shoulder2.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(L_Shoulder2.getPin(), L_Shoulder2.getValue(512, 3.2), 0);
  }
  if(L_Shoulder3.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(L_Shoulder3.getPin(), L_Shoulder3.getValue(512, 3.2), 0);
  }
  if(L_Elbow.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(L_Elbow.getPin(), L_Elbow.getValue(512, 3.2), 0);
  }
  if(R_Shoulder1.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(R_Shoulder1.getPin(), R_Shoulder1.getValue(512, 3.2), 0);
  }
  if(R_Shoulder2.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(R_Shoulder2.getPin(), R_Shoulder2.getValue(512, 3.2), 0);
  }
  if(R_Shoulder3.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(R_Shoulder3.getPin(), R_Shoulder3.getValue(512, 3.2), 0);
  }
  if(R_Elbow.updated() == true)
  {
    updated = true;
    Herkulex.moveAll(R_Elbow.getPin(), R_Elbow.getValue(512, 3.2), 0);
  }
  
  if(updated == true)
  { 
    unsigned long time = millis();
    unsigned long delta = millis() - preTime;
    if(delta > 1000)
      delta = 1;
    Herkulex.actionAll(delta);
    preTime = time;
  }
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  L_Shoulder1.reset();
  L_Shoulder2.reset();
  L_Shoulder3.reset();
  L_Elbow.reset();
  R_Shoulder1.reset();
  R_Shoulder2.reset();
  R_Shoulder3.reset();
  R_Elbow.reset();
  Herkulex.moveOneAngle(BROADCAST_ID, 0, 0, 0);
  
  preTime = millis();
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
  Herkulex.reboot(BROADCAST_ID);
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
