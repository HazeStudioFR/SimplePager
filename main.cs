using Rage;
using LSPD_First_Response.Mod.API;
using System.IO;
using System.Media;

namespace SimplePager
{
    public class Main : Plugin
    {
        public override void Finally() { }

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
        }

        static void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (onDuty)
            {
                GameFiber.StartNew(delegate
                {
                    Game.DisplayNotification( "SimplePager: ~p~loaded~w~!~n~Current Version: ~p~1.0.0");
                    bool hasPagerSounded = false;

                    while (onDuty)
                    {
                        GameFiber.Yield();
                        if (Functions.IsCalloutRunning())
                        {
                            string gameDirectory = Directory.GetCurrentDirectory();
                            string iniFileLocation = "\\plugins\\lspdfr\\SimplePager\\SFX_SOUND_PAGER.WAV";

                            if (File.Exists(gameDirectory + iniFileLocation) && !hasPagerSounded)
                            {
                                GameFiber.Wait(250);
                                SoundPlayer player = new SoundPlayer(gameDirectory + iniFileLocation);
                                player.Play();
                                hasPagerSounded = true;
                                GameFiber.Wait(5000);
                                player.Dispose();
                            }
                            else if (!File.Exists(gameDirectory + iniFileLocation))
                            {
                                Game.DisplayNotification("new_editor", "warningtriangle", "SimplePager", "[~y~WARNING~w~]", "The File:~n~SFX_SOUND_PAGER.WAV~n~Does Not Exist!");
                            }
                        }

                        if (!Functions.IsCalloutRunning() && hasPagerSounded)
                        {
                            hasPagerSounded = false;
                        }
                    }
                });
            }
        }
    }
}
