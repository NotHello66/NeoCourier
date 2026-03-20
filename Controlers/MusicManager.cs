using Godot;
using System.Collections.Generic;

public partial class MusicManager : Godot.AudioStreamPlayer
{
    [Export] public AudioStream[] exportedTracks = {};

    bool isPaused = false;
    List<AudioStream> tracks = new();
    int currentTrack = 0;

    public override void _Ready()
    {
        tracks = new List<AudioStream>(exportedTracks);
        if (tracks.Count > 0) PlayTrack(0);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Next music track") && tracks.Count > 0)
            PlayNextTrack();

        if (!Playing && !isPaused && tracks.Count > 0)
            PlayNextTrack();
    }

    private void PlayTrack(int index)
    {
        if (index < 0 || index >= tracks.Count) return;
        currentTrack = index;
        Stream = tracks[currentTrack];
        Play();
    }

    private void PlayNextTrack()
    {
        int next = (currentTrack + 1) % tracks.Count;
        PlayTrack(next);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Play or Pause music"))
        {
            isPaused = !isPaused;
            StreamPaused = isPaused;
        }
    }
}