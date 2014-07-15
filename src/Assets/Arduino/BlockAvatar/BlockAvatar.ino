#include <UnityRobot.h>
#include <ServoModule.h>
#include <Herkulex.h>

#define R_SHOULDER_PITCH_MIN 319
#define R_SHOULDER_PITCH_MAX 687
#define R_SHOULDER_ROLL_MIN 264
#define R_SHOULDER_ROLL_MAX 704
#define R_SHOULDER_YAW_MIN 512
#define R_SHOULDER_YAW_MAX 512
#define R_ELBOW_MIN 512
#define R_ELBOW_MAX 697
#define L_SHOULDER_PITCH_MIN 337
#define L_SHOULDER_PITCH_MAX 692
#define L_SHOULDER_ROLL_MIN 287
#define L_SHOULDER_ROLL_MAX 760
#define L_SHOULDER_YAW_MIN 512
#define L_SHOULDER_YAW_MAX 512
#define L_ELBOW_MIN 315
#define L_ELBOW_MAX 512

ServoModule R_Shoulder_Pitch(0, 0, -160, 160);
ServoModule R_Shoulder_Roll(1, 1, -160, 160);
ServoModule R_Shoulder_Yaw(2, 2, -160, 160);
ServoModule R_Elbow(3, 3, -160, 160);
ServoModule L_Shoulder_Pitch(4, 4, -160, 160);
ServoModule L_Shoulder_Roll(5, 5, -160, 160);
ServoModule L_Shoulder_Yaw(6, 6, -160, 160);
ServoModule L_Elbow(7, 7, -160, 160);

int cmdModule_id = 10;
byte cmdModule_value = 0;
boolean cmdModule_updated = false;
boolean cmdModule_start = false;
boolean cmdModule_flush = false;
unsigned long cmdModule_preTime;

unsigned long preTime = 0;

void OnUpdate(byte id)
{
  R_Shoulder_Pitch.update(id);
  R_Shoulder_Roll.update(id);
  R_Shoulder_Yaw.update(id);
  R_Elbow.update(id);
  L_Shoulder_Pitch.update(id);
  L_Shoulder_Roll.update(id);
  L_Shoulder_Yaw.update(id);
  L_Elbow.update(id);
  
  if(cmdModule_id == id)
  {
    UnityRobot.pop(&cmdModule_value);
    cmdModule_updated = true; 
  }
}

// When recieved end of update
void OnAction(void)
{
  boolean updated = false;
  int shoulder_pitch;
  int shoulder_roll;
  int shoulder_yaw;
  int elbow;
  
  //TODO: Synchronizing module's action
  if(R_Shoulder_Pitch.updated() == true)
  {
    updated = true;
    shoulder_pitch = R_Shoulder_Pitch.getValue(512, 3.2);
    Herkulex.moveAll(R_Shoulder_Pitch.getPin(), shoulder_pitch, 0);
  }
  if(R_Shoulder_Roll.updated() == true)
  {
    updated = true;
    shoulder_roll = R_Shoulder_Roll.getValue(512, 3.2);
    shoulder_roll = clamp(shoulder_roll, R_SHOULDER_ROLL_MIN, R_SHOULDER_ROLL_MAX);
    Herkulex.moveAll(R_Shoulder_Roll.getPin(), shoulder_roll, 0);
  }
  if(R_Shoulder_Yaw.updated() == true)
  {
    updated = true;
    shoulder_yaw = R_Shoulder_Yaw.getValue(512, 3.2);
    Herkulex.moveAll(R_Shoulder_Yaw.getPin(), shoulder_yaw, 0);
  }
  if(R_Elbow.updated() == true)
  {
    updated = true;
    elbow = R_Elbow.getValue(512, 3.2);
    Herkulex.moveAll(R_Elbow.getPin(), elbow, 0);
  }
  if(L_Shoulder_Pitch.updated() == true)
  {
    updated = true;
    shoulder_pitch = L_Shoulder_Pitch.getValue(512, 3.2);
    Herkulex.moveAll(L_Shoulder_Pitch.getPin(), shoulder_pitch, 0);
  }
  if(L_Shoulder_Roll.updated() == true)
  {
    updated = true;
    shoulder_roll = L_Shoulder_Roll.getValue(512, 3.2);
    shoulder_roll = clamp(shoulder_roll, L_SHOULDER_ROLL_MIN, L_SHOULDER_ROLL_MAX);
    Herkulex.moveAll(L_Shoulder_Roll.getPin(), shoulder_roll, 0);
  }
  if(L_Shoulder_Yaw.updated() == true)
  {
    updated = true;
    shoulder_yaw = L_Shoulder_Yaw.getValue(512, 3.2);
    Herkulex.moveAll(L_Shoulder_Yaw.getPin(), shoulder_yaw, 0);
  }
  if(L_Elbow.updated() == true)
  {
    updated = true;
    elbow = L_Elbow.getValue(512, 3.2);
    Herkulex.moveAll(L_Elbow.getPin(), elbow, 0);
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
  
  if(cmdModule_updated == true)
  {
    cmdModule_updated = false;
    cmdModule_start = true;
    cmdModule_preTime = millis();
  }
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  R_Shoulder_Pitch.reset();
  R_Shoulder_Roll.reset();
  R_Shoulder_Yaw.reset();
  R_Elbow.reset();
  L_Shoulder_Pitch.reset();
  L_Shoulder_Roll.reset();
  L_Shoulder_Yaw.reset();
  L_Elbow.reset();
  Herkulex.moveOneAngle(BROADCAST_ID, 0, 0, 0);
  
  cmdModule_value = 0;
  cmdModule_updated = false;
  
  preTime = millis();
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
}

void OnReady(void)
{
  if(cmdModule_flush == true)
  {
    UnityRobot.select(cmdModule_id);
    UnityRobot.push(cmdModule_value);
    UnityRobot.flush();
    cmdModule_flush = false;
  }
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
  
  if(cmdModule_start == true)
  {
    if(millis() - cmdModule_preTime > 3000)
    {
      cmdModule_start = false;
      cmdModule_value = 0;
      cmdModule_flush = true;
    }
  }
}

int clamp(int value, int min, int max)
{
  if(value < min)
    value = min;
  else if(value > max)
    value = max;
  
  return value;
}
