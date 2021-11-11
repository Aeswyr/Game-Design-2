using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PaletteAtlas", menuName = "Greed/PaletteAtlas", order = 0)]
public class PaletteAtlas : ScriptableObject {
    [SerializeField] private List<Color> colors;
}
