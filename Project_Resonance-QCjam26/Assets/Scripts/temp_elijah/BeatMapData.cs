using System;
using System.Collections.Generic;

namespace RhythmGame
{
    // Raw, serializable beatmap format. This is what you'd load from JSON.
    // Positions are expressed in "ticks" rather than seconds/beats directly,
    // so any subdivision (1/16, 1/8, triplets, etc.) is just an integer tick value.
    [Serializable]
    public class BeatMapData
    {
        public string songName;

        // Resolution of the tick grid. 96 or 192 per quarter note is common
        // (divides cleanly into halves, quarters, eighths, sixteenths, and triplets).
        public int ticksPerQuarterNote = 96;

        // BPM at tick 0. If the song never changes tempo, this is the only BPM you need.
        public float initialBpm = 120f;

        // Optional mid-song tempo changes, sorted by tick ascending.
        // Leave empty for constant-BPM songs.
        public List<TempoChange> tempoChanges = new List<TempoChange>();

        // The actual notes/events to hit, sorted by tick ascending.
        public List<NoteEntry> notes = new List<NoteEntry>();

        // Seconds of silence/lead-in before tick 0 actually plays. Useful if you want
        // a count-in or if the audio clip itself has silence baked in that you want to
        // account for explicitly rather than guess at.
        public float audioLeadInSeconds = 0f;
    }

    [Serializable]
    public class TempoChange
    {
        public int tick;
        public float bpm;
    }

    [Serializable]
    public class NoteEntry
    {
        public int tick;
        public int lane;       // which input lane/key this note belongs to
        public string noteType; // "tap", "hold", etc. -- extend as needed
    }
}
