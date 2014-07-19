#include <UnityRobot.h>
#include <ServoModule.h>
#include <Servo.h>

class PulseModule
{
  int _id;  
  word _duration;
  boolean _updated;
  unsigned long _time;
  int _off;
  int _on;
  boolean _do;
  
public:
  PulseModule(byte id, int onValue, int offValue);
  
  void update(byte id);
  void action();
  void reset();
  int process();  
};

// drum
PulseModule pulse1(1, 130, 80);
#define SERVO1_PIN 16
Servo servo1;
// bell
PulseModule pulse2(2, 130, 90);
#define SERVO2_PIN 17
Servo servo2;
// cymbals
PulseModule pulse3(3, 90, 70); 
#define SERVO3_PIN 18
Servo servo3;
// wood block
PulseModule pulse4(4, 130, 80);
Servo servo4;
#define SERVO4_PIN 19
// rhythm
PulseModule pulse5(5, 120, 60);
Servo servo5;
#define SERVO5_PIN 20
// thunder
PulseModule pulse6(6, 130, 50);
Servo servo6;
#define SERVO6_PIN 21


void OnUpdate(byte id)
{
  pulse1.update(id);
  pulse2.update(id);
  pulse3.update(id);
  pulse4.update(id);
  pulse5.update(id);
  pulse6.update(id);
}

// When recieved end of update
void OnAction(void)
{
  //TODO: Synchronizing module's action
  pulse1.action();
  pulse2.action();
  pulse3.action();
  pulse4.action();
  pulse5.action();
  pulse6.action();
}

// When recieved start of connection
void OnStart(void)
{
  //TODO: Initialize argument of module
  pulse1.reset();
  pulse2.reset();
  pulse3.reset();
  pulse4.reset();
  pulse5.reset();
  pulse6.reset();
}

// When recieved exit of connection
void OnExit(void)
{
  //TODO: Initialize argument of module
  pulse1.reset();
  pulse2.reset();
  pulse3.reset();
  pulse4.reset();
  pulse5.reset();
  pulse6.reset();
}

void OnReady(void)
{
}

void setup()
{
  servo1.attach(SERVO1_PIN);
  servo2.attach(SERVO2_PIN);
  servo3.attach(SERVO3_PIN);
  servo4.attach(SERVO4_PIN);
  servo5.attach(SERVO5_PIN);
  servo6.attach(SERVO6_PIN);
  
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
  
  servo1.write(pulse1.process());
  servo2.write(pulse2.process());
  servo3.write(pulse3.process());
  servo4.write(pulse4.process());
  servo5.write(pulse5.process());
  servo6.write(pulse6.process());
}
