using Pepperon.Scripts.Networking.Services;
using UnityEngine;

namespace Pepperon.Scripts.DI {
public class G: MonoBehaviour {
    public static G Instance { get; private set; }

    public MatchService matchService;
    
    private void Awake() {
        Instance = this;
        matchService = new MatchService();
    }
}
}