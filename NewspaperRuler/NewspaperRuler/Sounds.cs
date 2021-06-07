using System;
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
        private readonly WindowsMediaPlayer printingMachine = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer beginLevel = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer cancel = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer title = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer mainMenu = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer suddenness = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer gameOver = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer menuButton = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer finalMusic1 = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer finalMusic2 = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer end = new WindowsMediaPlayer();

        private bool playMusicLoop;
        private bool playMainMenuLoop;
        private bool playFinalMusic1Loop;
        private bool playFinalMusic2Loop;
        private bool playEndLoop;

        public Sounds()
        {
            music.settings.volume = 60;
            finalMusic1.settings.volume = 60;
            finalMusic2.settings.volume = 60;
            end.settings.volume = 50;
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
            printingMachine.URL = @"Sounds\Print.wav";
            beginLevel.URL = @"Sounds\BeginLevel.wav";
            beginLevel.settings.volume = 60;
            cancel.URL = @"Sounds\Cancel.wav";
            title.URL = @"Sounds\Title.wav";
            mainMenu.URL = @"Sounds\MainMenu.wav";
            mainMenu.settings.volume = 40;
            suddenness.URL = @"Sounds\Suddenness.wav";
            gameOver.URL = @"Sounds\GameOver.wav";
            menuButton.URL = @"Sounds\MenuButton.wav";
            finalMusic1.URL = @"Sounds\FinalMusic1.wav";
            finalMusic2.URL = @"Sounds\FinalMusic2.wav";
            end.URL = @"Sounds\End.wav";
            StopAll();
        }

        public void StopAll()
        {
            finalMusic2.close();
            finalMusic1.close();
            menuButton.close();
            gameOver.close();
            suddenness.close();
            mainMenu.close();
            title.close();
            cancel.close();
            beginLevel.close();
            panelHide.close();
            printingMachine.close();
            panelShow.close();
            notification.close();
            paper.close();
            chooseOption.close();
            stampEnter.close();
            stampPut.close();
            music.close();
            end.close();
            foreach (var sound in stampTake) sound.close();
            foreach (var sound in stampReturn) sound.close();
            playFinalMusic1Loop = false;
            playFinalMusic2Loop = false;
            playMainMenuLoop = false;
            playMusicLoop = false;
            playEndLoop = false;
        }

        public void EveryTick()
        {
            if (playMusicLoop && music.playState != WMPPlayState.wmppsPlaying)
                Play(music);
            if (playMainMenuLoop 
                && mainMenu.playState != WMPPlayState.wmppsPlaying
                && title.playState != WMPPlayState.wmppsPlaying)
                Play(mainMenu);
            if (playFinalMusic1Loop && finalMusic1.playState != WMPPlayState.wmppsPlaying)
                Play(finalMusic1);
            if (playFinalMusic2Loop && finalMusic2.playState != WMPPlayState.wmppsPlaying)
                Play(finalMusic2);
            if (playEndLoop && end.playState != WMPPlayState.wmppsPlaying)
                Play(end);
        }

        public void PlayMusic() => playMusicLoop = true;

        public void PlayMainMenu() => playMainMenuLoop = true;

        public void PlayFinalMusic1() => playFinalMusic1Loop = true;

        public void PlayFinalMusic2() => playFinalMusic2Loop = true;

        public void PlayEnd() => playEndLoop = true;

        public void StopMusic()
        {
            music.close();
            playMusicLoop = false;
        }

        public void StopMainMenu()
        {
            mainMenu.close();
            playMainMenuLoop = false;
        }

        public void StopFinalMusic1()
        {
            finalMusic1.close();
            playFinalMusic1Loop = false;
        }

        public void StopFinalMusic2()
        {
            finalMusic2.close();
            playFinalMusic2Loop = false;
        }

        public void StopEnd()
        {
            end.close();
            playEndLoop = false;
        }

        public void PlayTitle() => Play(title);

        public void StopTitle() => title.close();

        public void PlayStampEnter() => Play(stampEnter);

        public void PlayStampTake() => PlayRandom(stampTake);

        public void PlayStampReturn() => PlayRandom(stampReturn);

        public void PlayStampPut() => Play(stampPut);

        public void PlayPaper() => Play(paper);

        public void PlayChooseOption() => Play(chooseOption);

        public void PlayNotification() => Play(notification);

        public void PlayPanelShow() => Play(panelShow);

        public void PlayPanelHide() => Play(panelHide);

        public void StopPanelHide() => panelHide.close();

        public void PlayPrintingMachine() => Play(printingMachine);

        public void PlayBeginLevel() => Play(beginLevel);

        public void PlayCancel() => Play(cancel);

        public void PlaySuddenness() => Play(suddenness);

        public void PlayGameOver() => Play(gameOver);

        public void PlayMenuButton() => Play(menuButton);

        private void Play(WindowsMediaPlayer sound) => sound?.controls.play();

        private void PlayRandom(WindowsMediaPlayer[] sounds) => sounds[random.Next(sounds.Length)]?.controls.play();
    }
}
