﻿using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;

/// <summary>  
/// - Handles MIDI device 
/// </summary>  
public class MidiController : MonoBehaviour
{
    protected InputDevice inputDevice;

    private List<MidiEventStorage> midiEvents;

    void Start()
    {
        if (InputDevice.DeviceCount != 1)
        {
            Debug.LogError("Err: No device or too many devices found for MIDI input.");
            throw new System.Exception();
        }
        inputDevice = new InputDevice(0);
        inputDevice.ChannelMessageReceived += handleChannelMsg;
        inputDevice.ChannelMessageReceived += storeMidiEvent;

        inputDevice.StartRecording();
        Debug.Log("MIDI device inited");
        clearMidiEventStorage();
    }

    public void clearMidiEventStorage()
    {
        Debug.Log("Clearing MIDI events storage");
        midiEvents = new List<MidiEventStorage>();
    }

    public List<MidiEventStorage> GetMidiEvents()
    {
        return midiEvents;
    }

    void OnApplicationQuit()
    {
        Debug.LogWarning("MIDI closed");
        inputDevice.Dispose();
    }

    void handleChannelMsg(object sender, ChannelMessageEventArgs e)
    {
        var keyNum = e.Message.Data1;
        // Handle MIDI event

        Debug.Log(e.Message.Command.ToString() + '\t' + '\t' + e.Message.MidiChannel.ToString() + '\t' + keyNum.ToString() + '\t' + e.Message.Data2.ToString());
        if (e.Message.Command == ChannelCommand.NoteOn)
        {
            PianoBuilder.instance.ActivateKey(keyNum, Color.green);
        }
        else if (e.Message.Command == ChannelCommand.NoteOff)
        {
            PianoBuilder.instance.DeactivateKey(keyNum);
        }
    }

    void storeMidiEvent(object sender, ChannelMessageEventArgs e)
    {
        midiEvents.Add(new MidiEventStorage(e, Time.time));
    }

}

public struct MidiEventStorage
{
    public float time { get; }

    public int keyNum { get; }

    public bool isEnd { get; }

    public MidiEventStorage(ChannelMessageEventArgs e, float time)
    {
        this.time = time;
        this.keyNum = e.Message.Data1;
        this.isEnd = e.Message.Command == ChannelCommand.NoteOff;
    }
}


