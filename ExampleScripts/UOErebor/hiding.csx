using System;
using System.Linq;

public static class Hiding
{
    // Create a "private" instance of journal for cooking. If you delete this journal it
    // doesn't affect either UO.Journal or other instances of SpeechJournal so
    // you can try hiding and healing in parallel (use bandage, target self, start hiding)
    // and you can still use journal.Delete method in both scripts at the same time.
    // It means, that you don't need tricks like UO.SetJournalLine(number,text) in
    // Injection.
    private static SpeechJournal hidingJournal = UO.CreateSpeechJournal();
    
    public static bool AlwaysWalkEnabled { get; set; } = false;
    public static TimeSpan AlwaysWalkDelayTime { get; set; } = TimeSpan.FromMilliseconds(1800);
    public static ScriptTrace Trace = UO.Trace.Create();

    public static void Hide()
    {
        UO.ClientFilters.Stamina.Disable();

        if (UO.CommandHandler.IsCommandRunning("hide-run"))
        {
            UO.ClientPrint("Hiding stopped", "hiding", UO.Me);
            UO.CommandHandler.Terminate("hide-run");
        }
        else
        {
            UO.CommandHandler.Invoke("hide-run");
            if (!UO.CommandHandler.IsCommandRunning("hide-watchalwayswalk"))
                UO.CommandHandler.Invoke("hide-watchalwayswalk");
        }
    }
    
    private static SpeechJournal alwaysWalkJournal = UO.CreateSpeechJournal();
    
    public static void RunWatchAlwaysWalk()
    {
        while (true)
        {
            alwaysWalkJournal.WaitAny(TimeSpan.MaxValue,
                "Byla jsi objevena",
                "Byl jsi objeven",
                "You have been revealed!",
                "You're now visible.");
                
            UO.Trace.Log("disabling always walk");
            UO.ClientFilters.Stamina.Disable();
        }
    }

    public static void RunHiding()
    {
        try
        {
            bool hidden = false;
            do
            {
                UO.ClientPrint("Trying to hide", "hiding", UO.Me);
                
                if (UO.Me.IsHidden)
                {
                    UO.ClientPrint("Already hidden");
                    return;
                }
                
                // Don't worry, it will not affect any other scripts.
                UO.WarModeOff();
                hidingJournal.Delete();
                UO.UseSkill(Skill.Hiding);
                
                bool unfinishedAttempt = false;
                            
                do 
                {
                    Trace.Log($"hide cycle, {DateTime.UtcNow:mm:ss:fffff}, unfinishedAttempt: {unfinishedAttempt}"); 
                    unfinishedAttempt = false;
                    // This waits until "Skryti se povedlo." or "Nepovedlo se ti schovat" arrives to journal.
                    hidingJournal
                        .When("You must wait a few moments to use another skill", () =>
                            // wait when you have to wait before using another script
                            UO.Wait(5000))
                        .When("Skryti se povedlo.", () =>
                            // when hiding is successful terminate the do while loop
                            // (you cannot use break statement directly in an annonynous method)
                            hidden = true)
                        .When("Nepovedlo se ti schovat", () =>
                        {
                            // when hiding fails, do while loop continues
                            hidden = false;
                            UO.ClientFilters.Stamina.Disable();
                            Trace.Log("enabling always walk");
                        })
                        .When("You are preoccupied with thoughts of battle.", () =>
                        {
                            UO.WarModeOff();
                            hidden = false;
                        })
                        .WhenTimeout(() =>
                        {
                            Trace.Log($"hide timeout for always walk after {AlwaysWalkDelayTime}, {DateTime.UtcNow:mm:ss:fffff}");
                            if (AlwaysWalkEnabled)
                            {
                                Trace.Log("enabling always walk");
                                UO.ClientFilters.Stamina.SetFakeStamina(0);
                            }
                            
                            unfinishedAttempt = true;
                        })
                        // if server sends neither "Skryti se povedlo." nor "Nepovedlo se ti schovat"
                        // in one minute, then the script terminates with an TimoutException.
                        .WaitAny(AlwaysWalkDelayTime);
                } while (unfinishedAttempt);
            } while (!hidden);
        }
        finally
        {
            UO.ClientFilters.Stamina.Disable();
        }
    }
}

UO.RegisterCommand("hide", Hiding.Hide);
UO.RegisterBackgroundCommand("hide-run", Hiding.RunHiding);
UO.RegisterBackgroundCommand("hide-watchalwayswalk", Hiding.RunWatchAlwaysWalk);
