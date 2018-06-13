﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using Melanchall.DryWetMidi.Common;
using System.Linq;

/// <summary>  
/// - Builds piano roll from MIDI file after virtual piano is built
/// </summary>  
[RequireComponent(typeof(Piano))]
public class Sequencer : MonoBehaviour
{
    public string midiFileName;

    public float pianoRollSpeed;

    private MidiFile midiFile;

    private NotesManager noteManager;

    internal Piano piano;

    [SerializeField]
    public GameObject dropDowns;

    private List<GameObject> pianoRollObjects = new List<GameObject>();

    public static Sequencer instance;
    void Start()
    {
        this.piano = GetComponent<Piano>();
        instance = this;
        midiFile = MidiFile.Read(midiFileName);
    }

    public void spawnNotes()
    {
        if (noteManager == null)
        {
            for (int i = 0; i < midiFile.Chunks.Count; i++)
            {
                MidiChunk chunk = midiFile.Chunks[i];
                if (chunk.GetType().Equals(typeof(TrackChunk)))
                {
                    using (var nm = new NotesManager(((TrackChunk)chunk).Events))
                    {
                        this.noteManager = nm;
                    }
                    break;
                }
            }
        }

        SpawnNotesDropDown(noteManager.Notes.ToList());
    }

    private void ClearPianoRoll()
    {
        Debug.Log("Clearing piano roll");
        pianoRollObjects.ForEach(o => GameObject.Destroy(o));
        pianoRollObjects.Clear();
    }

    public void SpawnNotesDropDown(List<Note> notes)
    {
        ClearPianoRoll();
        Debug.Log("Spawning piano roll");
        notes.ForEach(e =>
        {
            var number = e.NoteNumber;
            var start = e.Time;
            var dur = e.Length;
            float x, y, z;
            var key = PianoKeys.GetKeyFor(number);
            if (key == null)
            {
                return;
            }
            y = start / 1000f;
            var scale = e.Length / 1000f - 0.01f;
            piano.GetOffsetForKeyNum(number, out x, out z);
            var obj = Instantiate(dropDowns);
            pianoRollObjects.Add(obj);
            var dropdownScale = obj.transform.localScale;
            obj.transform.localScale = new Vector3(dropdownScale.x, scale, dropdownScale.z);
            obj.transform.position = new Vector3(x + 0.0015f, y + 1, z);
            var renderer = obj.GetComponent<Renderer>();
            renderer.material.color = key.color == KeyColor.Black ? Color.black : Color.white;
            var rb = obj.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(0, -pianoRollSpeed, 0);
        });

    }

}