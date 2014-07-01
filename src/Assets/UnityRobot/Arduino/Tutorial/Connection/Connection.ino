#include <UnityRobot.h>

void setup()
{
  UnityRobot.begin(57600);
}

void loop()
{
  UnityRobot.process();
}
