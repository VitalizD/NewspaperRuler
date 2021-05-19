﻿using System;
using System.Reflection;
using WMPLib;

namespace NewspaperRuler
{
    public class Sounds
    {
        private readonly Random random = new Random();
        private readonly WindowsMediaPlayer music = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer stampPut = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer[] stampTake = new WindowsMediaPlayer[4];
        private readonly WindowsMediaPlayer[] stampReturn = new WindowsMediaPlayer[2];
        private readonly WindowsMediaPlayer stampEnter = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer chooseOption = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer paper = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer notification = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer panelShow = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer panelHide = new WindowsMediaPlayer();

        public Sounds()
        {
            music.settings.volume = 60;
            music.URL = @"Sounds\Music.wav";
            stampPut.URL = @"Sounds\StampPut.wav";
            for (var i = 0; i < stampTake.Length; i++) stampTake[i] = new WindowsMediaPlayer();
            stampTake[0].URL = @"Sounds\StampTake1.wav";
            stampTake[1].URL = @"Sounds\StampTake2.wav";
            stampTake[2].URL = @"Sounds\StampTake3.wav";
            stampTake[3].URL = @"Sounds\StampTake4.wav";
            for (var i = 0; i < stampReturn.Length; i++) stampReturn[i] = new WindowsMediaPlayer();
            stampReturn[0].URL = @"Sounds\StampReturn1.wav";
            stampReturn[1].URL = @"Sounds\StampReturn2.wav";
            stampEnter.URL = @"Sounds\StampEnter.wav";
            chooseOption.URL = @"Sounds\ChooseOption.wav";
            paper.URL = @"Sounds\Paper.wav";
            notification.URL = @"Sounds\Notification.wav";
            panelShow.URL = @"Sounds\PanelShow.wav";
            panelHide.URL = @"Sounds\PanelHide.wav";
        }

        public void Tick()
        {
            if (music.playState != WMPPlayState.wmppsPlaying) 
                music.controls.play();
        }

        public void StampEnter() => Play(stampEnter);

        public void StampTake() => PlayRandom(stampTake);

        public void StampReturn() => PlayRandom(stampReturn);

        public void StampPut() => Play(stampPut);

        public void Paper() => Play(paper);

        public void ChooseOption() => Play(chooseOption);

        public void Notification() => Play(notification);

        public void PanelShow() => Play(panelShow);

        public void PanelHide() => Play(panelHide);

        private void Play(WindowsMediaPlayer sound) => sound.controls.play();

        private void PlayRandom(WindowsMediaPlayer[] sounds) => sounds[random.Next(sounds.Length)].controls.play();
    }
}
