using UnityEngine;

public class FactionMember : MonoBehaviour
{
    [SerializeField] private FactionId factionId;
    public FactionId GetFactionId => factionId;

    public void SetFaction(FactionId id) => factionId = id;
}
