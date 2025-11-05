using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroViewCreator : Singleton<HeroViewCreator>
{
    [SerializeField] private HeroView heroViewPrefab;
    public HeroView CreateHeroView(HeroData heroData, Vector3 position, Vector3 playerposition, Quaternion rotation)
    {
        HeroView heroView = Instantiate(heroViewPrefab, playerposition, rotation);
        //heroView.spriteRenderer.transform.localScale = new Vector3(0.25f, 0.25f);
        heroView.SetUp(heroData);
        return heroView;
    }
}
