﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IrrKlang;
using System.Threading;
using Spoffice.Website.Models;

namespace Spoffice.Website.Services.Music.Player
{
    public class IrrklangMusicPlayer : IMusicPlayer
    {
        public long TotalBytes { get; set; }
        private ISoundEngine engine;
        public IrrklangMusicPlayer(long bytesPlayed)
        {
            TotalBytes = bytesPlayed;
            engine = new ISoundEngine();
        }
        #region IMusicPlayer Members

        public void PlayTrack(Track track)
        {
            // stop any current sounds just to make sure we don't hurt peoples ears
            engine.StopAllSounds();

            track.Progress = 0;
            Thread thread = new Thread(new ParameterizedThreadStart(DoPlay));
            thread.Start(track);
        }

        #endregion

        public void DoPlay(object t)
        {
            Track track = (Track)t;

            // sleep for a second. don't ask me why. I forgot why.
            Thread.Sleep(1000);

            ISound sound = engine.Play2D(track.CachePath);
            track.Length = (int)sound.PlayLength;
            long startTotalBytes = TotalBytes;
            while (!sound.Finished)
            {
                track.Progress = (int)sound.PlayPosition;
                TotalBytes = startTotalBytes + Convert.ToInt32(((double)track.BytesTotal) * ((double)sound.PlayPosition / (double)sound.PlayLength));
                System.Threading.Thread.Sleep(500);
            }

            track.OnPlayed();
        }
    }
}