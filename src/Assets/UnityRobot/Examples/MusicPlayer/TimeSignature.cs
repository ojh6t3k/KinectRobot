using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TimeSignature
{

    public enum NoteDuration
    {
        ThirtySecond,
        Sixteenth,
        Triplet,
        Eighth,
        DottedEighth,
        Quarter,
        DottedQuarter,
        Half,
        DottedHalf,
        Whole
    };

    public int numerator;      /** Numerator of the time signature */
    public int denominator;    /** Denominator of the time signature */
    public int quarternote;    /** Number of pulses per quarter note */
    public int measure;        /** Number of pulses per measure */
    public int tempo;          /** Number of microseconds per quarter note */

    /** Get the numerator of the time signature */
    public int Numerator
    {
        get { return numerator; }
    }

    /** Get the denominator of the time signature */
    public int Denominator
    {
        get { return denominator; }
    }

    /** Get the number of pulses per quarter note */
    public int Quarter
    {
        get { return quarternote; }
    }

    /** Get the number of pulses per measure */
    public int Measure
    {
        get { return measure; }
    }

    /** Get the number of microseconds per quarter note */
    public int Tempo
    {
        get { return tempo; }
    }

    /** Create a new time signature, with the given numerator,
        * denominator, pulses per quarter note, and tempo.
        */
    public TimeSignature(int numerator, int denominator, int quarternote, int tempo)
    {
        if (numerator <= 0 || denominator <= 0 || quarternote <= 0)
        {
            throw new MidiFileException("Invalid time signature", 0);
        }

        /* Midi File gives wrong time signature sometimes */
        if (numerator == 5)
        {
            numerator = 4;
        }

        this.numerator = numerator;
        this.denominator = denominator;
        this.quarternote = quarternote;
        this.tempo = tempo;

        int beat;
        if (denominator == 2)
            beat = quarternote * 2;
        else
            beat = quarternote / (denominator / 4);

        measure = numerator * beat;
    }

    /** Return which measure the given time (in pulses) belongs to. */
    public int GetMeasure(int time)
    {
        return time / measure;
    }

    /** Given a duration in pulses, return the closest note duration. */
    public NoteDuration GetNoteDuration(int duration)
    {
        int whole = quarternote * 4;

        /**
            1       = 32/32
            3/4     = 24/32
            1/2     = 16/32
            3/8     = 12/32
            1/4     =  8/32
            3/16    =  6/32
            1/8     =  4/32 =    8/64
            triplet         = 5.33/64
            1/16    =  2/32 =    4/64
            1/32    =  1/32 =    2/64
            **/

        if (duration >= 28 * whole / 32)
            return NoteDuration.Whole;
        else if (duration >= 20 * whole / 32)
            return NoteDuration.DottedHalf;
        else if (duration >= 14 * whole / 32)
            return NoteDuration.Half;
        else if (duration >= 10 * whole / 32)
            return NoteDuration.DottedQuarter;
        else if (duration >= 7 * whole / 32)
            return NoteDuration.Quarter;
        else if (duration >= 5 * whole / 32)
            return NoteDuration.DottedEighth;
        else if (duration >= 6 * whole / 64)
            return NoteDuration.Eighth;
        else if (duration >= 5 * whole / 64)
            return NoteDuration.Triplet;
        else if (duration >= 3 * whole / 64)
            return NoteDuration.Sixteenth;
        else
            return NoteDuration.ThirtySecond;
    }

    /** Convert a note duration into a stem duration.  Dotted durations
        * are converted into their non-dotted equivalents.
        */
    public NoteDuration GetStemDuration(NoteDuration dur)
    {
        if (dur == NoteDuration.DottedHalf)
            return NoteDuration.Half;
        else if (dur == NoteDuration.DottedQuarter)
            return NoteDuration.Quarter;
        else if (dur == NoteDuration.DottedEighth)
            return NoteDuration.Eighth;
        else
            return dur;
    }

    /** Return the time period (in pulses) the the given duration spans */
    public int DurationToTime(NoteDuration dur)
    {
        int eighth = quarternote / 2;
        int sixteenth = eighth / 2;

        switch (dur)
        {
            case NoteDuration.Whole:
                return quarternote * 4;
            case NoteDuration.DottedHalf:
                return quarternote * 3;
            case NoteDuration.Half:
                return quarternote * 2;
            case NoteDuration.DottedQuarter:
                return 3 * eighth;
            case NoteDuration.Quarter:
                return quarternote;
            case NoteDuration.DottedEighth:
                return 3 * sixteenth;
            case NoteDuration.Eighth:
                return eighth;
            case NoteDuration.Triplet:
                return quarternote / 3;
            case NoteDuration.Sixteenth:
                return sixteenth;
            case NoteDuration.ThirtySecond:
                return sixteenth / 2;
            default: return 0;
        }
    }

    public override
    string ToString()
    {
        return string.Format("TimeSignature={0}/{1} quarter={2} tempo={3}",
                                numerator, denominator, quarternote, tempo);
    }

}


