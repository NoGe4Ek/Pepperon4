using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Controllers;
using Pepperon.Scripts.Entities.Systems.LoreSystem.Base.Infos;

namespace Pepperon.Scripts.Entities.Components {
public class ExperienceComponent: NetworkBehaviour {
    private ExperienceInfo experienceInfo;
    [SyncVar] public int currentExperience = 0;
    [SyncVar] public int currentLevel = 1;
    private int nextLevelThresholdIndex = 1;

    private void Start() {
        experienceInfo = GetComponent<EntityController>().entity.info.OfType<ExperienceInfo>().First();
    }

    public void TakeExperience(int experience) {
        if (currentExperience >= experienceInfo.levelThresholds[^1]) return;
        currentExperience += experience;
        
        if (currentExperience < experienceInfo.levelThresholds[nextLevelThresholdIndex]) return;
        
        for (var i = nextLevelThresholdIndex; i < experienceInfo.levelThresholds.Length - 1; i++) {
            if (currentExperience < experienceInfo.levelThresholds[nextLevelThresholdIndex]) break;
            currentLevel++;
            nextLevelThresholdIndex++;
        }
    }
    
    public float GetCurrentExperience() => currentExperience;
}
}