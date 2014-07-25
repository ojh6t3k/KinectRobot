/*
  ToneModule.h - UnityRobot Module library
  Copyright (C) 2014 Robolink.  All rights reserved.
*/


//******************************************************************************
//* Includes
//******************************************************************************
#include "UnityRobot.h"
#include "ToneModule.h"


//******************************************************************************
//* Constructors
//******************************************************************************
ToneModule::ToneModule(int id)
{	
	_id = id;
	_pin = -1;
	reset();
}

ToneModule::ToneModule(int id, int pin)
{	
	_id = id;
	_pin = pin;
	reset();
}

//******************************************************************************
//* Public Methods
//******************************************************************************

void ToneModule::reset()
{
	_updated = false;
#if !defined(Servo_h)
	mute();
#endif
}

void ToneModule::update(int id)
{
	if(id == _id)
	{
		UnityRobot.pop(&_note);		
		_updated = true;
	}
}

void ToneModule::action()
{
	if(_pin != -1)
	{
#if !defined(Servo_h)
		if(_note == NOTE_MUTE)
			mute();
		else
			tone(_pin, _note);
#endif
	}
}

boolean ToneModule::updated()
{
	boolean res = _updated;
	_updated = false;
	return res;
}

void ToneModule::mute()
{
	_note = NOTE_MUTE;
	if(_pin != -1)
	{
#if !defined(Servo_h)
		noTone(_pin);
#endif		
	}
}

int ToneModule::getNote()
{	
	return (int)_note;
}

//******************************************************************************
//* Private Methods
//******************************************************************************

