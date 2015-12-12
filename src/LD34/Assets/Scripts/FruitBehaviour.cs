using UnityEngine;
using System.Collections;

public class FruitBehaviour : PickUpBehaviour
{
    public Sprite[] FruitSprites;
    private SpriteRenderer _spriteRenderer;

	// Use this for initialization
	public override void Start ()
	{
        _spriteRenderer = GetComponent<SpriteRenderer>();

	    var fruitType = Random.Range(0, FruitSprites.Length - 1);

	    _spriteRenderer.sprite = FruitSprites[fruitType];
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
