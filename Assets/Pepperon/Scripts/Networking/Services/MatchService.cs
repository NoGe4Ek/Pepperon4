using System;
using System.Collections.Generic;
using Pepperon.Scripts.Managers;
using Pepperon.Scripts.UI;
using Pepperon.Scripts.Utils;
using UnityEngine;

namespace Pepperon.Scripts.Networking.Services {
public class MatchService {
    public static void EndMatch(MatchResult result) {
        var endMatchRequest = HttpClient.Instance.Post<EmptyResponse>(
            "https://www.aphirri.ru/matches/end/" + BootManager.Instance.MatchId,
            result,
            response => { Debug.Log("Response: " + response); },
            error => { Debug.Log("Error: " + error); });
        new Task(endMatchRequest);
    }
}
}