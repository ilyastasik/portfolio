using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class BazaUITest : MonoBehaviour
{
    public Mesh mesh_quad;
    public Material material;
    [Range(1,100)]
    public int n;
    public bool instanced;
    public int renderLayer;
    public uint renderLayerMask;
    public Font font;
    public Material fontMaterial;
    public Material buttonInstancedMaterial;
    public float distance;
    public Sprite buttonSprite;

    public int colorPropID;
    MaterialPropertyBlock propBlock;
    RenderParams rp;
    RenderParams fontRenderParams;

    ButtonInstanceData[] buttons;

    [Range(1,10000)]
    public int instancesCount = 1;

    [Button]
    void Refresh() {
        OnEnable();
    }

    public struct ButtonInstanceData {
        public Matrix4x4 objectToWorld;
    }

    public Matrix4x4[] matrices;

    private void OnValidate() {
        OnEnable();
    }

    void OnEnable() {
        Application.targetFrameRate= 60;

        Debug.Log("Button sprite: " + buttonSprite.border);
        Debug.Log($"Texture: {buttonSprite.texture}, {buttonSprite.textureRect}");

        //QualitySettings.;
        colorPropID = Shader.PropertyToID("_Color");
        //colorPropID = material.shader.FindPropertyIndex("_Color");
        propBlock = new MaterialPropertyBlock();
        rp = new RenderParams(material);
        fontRenderParams = new RenderParams(fontMaterial);
        //font.rect

        string[] fonts = Font.GetOSInstalledFontNames();
        Debug.Log($"Installed fonts: {fonts.Length}\n{string.Join('\n',fonts)}");
        Debug.Log($"Font info: characters - {font.characterInfo.Length}");
        Debug.Log(string.Join('\n', font.characterInfo.Select(x => $"u{x.index};{x.uv};").ToList()));

        buttons = new ButtonInstanceData[instancesCount];
        matrices = new Matrix4x4[instancesCount];

        char ch = 'ß';
        font.GetCharacterInfo(ch, out CharacterInfo charInfo);
        Debug.Log($"charInfo {ch}: flipped {charInfo.flipped}");

        rp = new RenderParams(buttonInstancedMaterial);
        rp.matProps = new MaterialPropertyBlock();
        List<Vector4> colors = new List<Vector4>();
        List<Vector4> uvs = new List<Vector4>();


        for (int i = 0; i < instancesCount; i++) {
            charInfo = font.characterInfo[i % font.characterInfo.Length];

            int sqr = (int)Mathf.Sqrt(instancesCount);

            int x = i / sqr;
            int y = i % sqr;

            if (charInfo.flipped) {
                buttons[i].objectToWorld = Matrix4x4.TRS(new Vector3(-4 + x * distance, -4 + y * distance), Quaternion.Euler(0,0,90), Vector3.one);
            } else {
                buttons[i].objectToWorld = Matrix4x4.Translate(new Vector3(-4 + x * distance, -4 + y * distance));
            }

            matrices[i] = buttons[i].objectToWorld;

            colors.Add(Random.ColorHSV());

            if (charInfo.flipped) {
                uvs.Add(new Vector4(charInfo.uvBottomLeft.x, charInfo.uvBottomLeft.y, charInfo.uvTopRight.x, charInfo.uvTopRight.y));
                //uvs.Add(new Vector4(charInfo.uvBottomLeft.y, charInfo.uvBottomLeft.x, charInfo.uvTopRight.y, charInfo.uvTopRight.x));
                //uvs.Add(new Vector4(charInfo.uvTopRight.x, charInfo.uvTopRight.y, charInfo.uvBottomLeft.x, charInfo.uvBottomLeft.y));
            } else {
                uvs.Add(new Vector4(charInfo.uvBottomLeft.x, charInfo.uvBottomLeft.y, charInfo.uvTopRight.x, charInfo.uvTopRight.y));
            }
        }

        rp.matProps.SetVectorArray("_Color", colors);
        rp.matProps.SetVectorArray("_uv", uvs);
    }

    void Update() {
        Draw(); 
    }

    void OnGUI() {
        if (GUILayout.Button("Text")) {

        }
    }

    void Draw() {
        if (instanced) {
            //propBlock.SetColor(colorPropID, Color.white);
            rp.rendererPriority = renderLayer;
            rp.layer = renderLayer;
            rp.renderingLayerMask = renderLayerMask;

            Graphics.RenderMeshInstanced<ButtonInstanceData>(rp, mesh_quad, 0, buttons, instancesCount, 0);
            //Graphics.DrawMeshInstanced(mesh_quad, 0, buttonInstancedMaterial, matrices, instancesCount, rp.matProps,
            //    UnityEngine.Rendering.ShadowCastingMode.Off, false, renderLayer, Camera.main, UnityEngine.Rendering.LightProbeUsage.Off);
        } else {
            for (int x = 0; x < n; x++) {
                for (int y = 0; y < n; y++) {
                    float sx = x / (float)n - 0.5f;
                    float sy = y / (float)n - 0.5f;

                    propBlock.SetColor(colorPropID, Color.white);
                    rp.matProps = propBlock;

                    Graphics.RenderMesh(rp, mesh_quad, 0, Matrix4x4.Translate(new Vector3(sx, sy, 0)));
                }
            }

            for (int i = 0; i < 100; i++) {
                DrawChar('H', new Vector2(0, 0));
                DrawChar('E', new Vector2(1, 0));
                DrawChar('L', new Vector2(2, 0));
            }
        }
    }

    void DrawChar(char letter, Vector2 pos) {
        font.GetCharacterInfo(letter, out CharacterInfo charInfo);

        Mesh mesh = new Mesh();

        mesh.SetVertices(new List<Vector3>() {
                new Vector3(0,0),
                new Vector3(0,1),
                new Vector3(1,1),
                new Vector3(1,0),
            });

        mesh.SetTriangles(new int[] {
                0,1,2,
                0,2,3,
            }, 0);

        mesh.SetColors(new List<Color>() {
                Color.white,
                Color.white,
                Color.white,
                Color.white
            });

        mesh.SetUVs(0, new List<Vector2>() {
                charInfo.uvBottomLeft,
                charInfo.uvTopLeft,
                charInfo.uvTopRight,
                charInfo.uvBottomRight
            });

        //Graphics.DrawMesh()
        Graphics.RenderMesh(fontRenderParams, mesh, 0, Matrix4x4.Translate(pos));
    }
}
