using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmGame
{
    // A single resolved note event: a tick position converted to an absolute
    // AudioSettings.dspTime, ready for scheduling/judging.
    public struct NoteEvent
    {
        public int tick;
        public double dspTime;
        public int lane;
        public string noteType;
    }

    // Runtime wrapper around BeatMapData. Handles converting tick positions to
    // absolute dspTime values, including support for mid-song tempo changes.
    public class BeatMap
    {
        public readonly BeatMapData data;
        public readonly double songStartDspTime; // dspTime at tick 0
        public readonly List<NoteEvent> notes;    // resolved, sorted by dspTime

        // Precomputed sections for tick -> seconds conversion.
        // Each section covers [startTick, nextSectionStartTick) with a constant secondsPerTick.
        private struct TempoSection
        {
            public int startTick;
            public double startTimeOffset; // seconds from songStartDspTime to this section's start
            public double secondsPerTick;
        }

        private readonly List<TempoSection> sections;

        public BeatMap(BeatMapData data, double songStartDspTime)
        {
            this.data = data;
            this.songStartDspTime = songStartDspTime;
            this.sections = BuildTempoSections(data);
            this.notes = ResolveNotes(data, sections, songStartDspTime);
        }

        private static List<TempoSection> BuildTempoSections(BeatMapData data)
        {
            var sections = new List<TempoSection>();

            // Merge initialBpm (at tick 0) with any tempoChanges, sorted ascending.
            var changes = new List<TempoChange> { new TempoChange { tick = 0, bpm = data.initialBpm } };
            changes.AddRange(data.tempoChanges.Where(c => c.tick > 0));
            changes = changes.OrderBy(c => c.tick).ToList();

            double cumulativeOffset = data.audioLeadInSeconds;
            for (int i = 0; i < changes.Count; i++)
            {
                double secondsPerTick = (60.0 / changes[i].bpm) / data.ticksPerQuarterNote;

                sections.Add(new TempoSection
                {
                    startTick = changes[i].tick,
                    startTimeOffset = cumulativeOffset,
                    secondsPerTick = secondsPerTick
                });

                // Advance cumulativeOffset by however many ticks this section spans
                // before the next tempo change begins.
                if (i < changes.Count - 1)
                {
                    int tickSpan = changes[i + 1].tick - changes[i].tick;
                    cumulativeOffset += tickSpan * secondsPerTick;
                }
            }

            return sections;
        }

        // Converts an absolute tick position into a dspTime.
        public double GetDspTimeForTick(int tick)
        {
            // Find the last section whose startTick <= tick.
            TempoSection section = sections[0];
            for (int i = 0; i < sections.Count; i++)
            {
                if (sections[i].startTick <= tick)
                    section = sections[i];
                else
                    break;
            }

            double secondsFromSongStart = section.startTimeOffset +
                (tick - section.startTick) * section.secondsPerTick;

            return songStartDspTime + secondsFromSongStart;
        }

        private static List<NoteEvent> ResolveNotes(BeatMapData data, List<TempoSection> sections, double songStartDspTime)
        {
            var result = new List<NoteEvent>(data.notes.Count);

            foreach (var entry in data.notes)
            {
                TempoSection section = sections[0];
                for (int i = 0; i < sections.Count; i++)
                {
                    if (sections[i].startTick <= entry.tick)
                        section = sections[i];
                    else
                        break;
                }

                double secondsFromSongStart = section.startTimeOffset +
                    (entry.tick - section.startTick) * section.secondsPerTick;

                result.Add(new NoteEvent
                {
                    tick = entry.tick,
                    dspTime = songStartDspTime + secondsFromSongStart,
                    lane = entry.lane,
                    noteType = entry.noteType
                });
            }

            return result.OrderBy(n => n.tick).ToList();
        }

        // Convenience: common subdivisions in ticks, given this beatmap's resolution.
        public int TicksPerQuarterNote => data.ticksPerQuarterNote;
        public int TicksPerEighthNote => data.ticksPerQuarterNote / 2;
        public int TicksPerSixteenthNote => data.ticksPerQuarterNote / 4;
        public int TicksPerWholeNote => data.ticksPerQuarterNote * 4;
        public int TicksPerTripletEighth => data.ticksPerQuarterNote / 3;
    }
}
