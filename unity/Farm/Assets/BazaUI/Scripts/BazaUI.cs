using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BazaUI : MonoBehaviour
{
    [SerializeField] MeshFactory meshFactory;

    [SerializeField] Material material_text;
    [SerializeField] Material material_image;
    [SerializeField] Font font;

    public Vector4 t0;
    public Vector4 t1;

    Sprite[] sprites;
    Dictionary<string, Sprite> spritesDict;

    List<WindowNode> windows;

    Node lastNode;
    bool didInit;

    Matrix4x4[] imageMatrices = new Matrix4x4[1024];
    Matrix4x4[] textMatrices = new Matrix4x4[1024];

    RenderParams imageParams;
    RenderParams textParams;

    List<Vector4> imagesColors;
    List<Vector4> imagesUvs;

    List<Vector4> textColors;
    List<Vector4> textUvs;

    int depth;
    int textIndex;
    int imageIndex;

    public float depthStep = -0.01f;



    [Button]
    void Generate() {

    }

    private void Start() {
        Init();
    }

    private void Update() {
        Tick();
    }

    private void OnEnable() {
        didInit = false;
        Init();
    }

    void Init() {
        didInit = true;

        sprites = Resources.LoadAll<Sprite>("bazaui");
        spritesDict = new Dictionary<string, Sprite>();
        foreach (var sprite in sprites) {
            spritesDict[sprite.name] = sprite;
        }

        meshFactory.Init();

        imageParams = new RenderParams(material_image);
        imageParams.matProps = new MaterialPropertyBlock();

        textParams = new RenderParams(material_text);
        textParams.matProps = new MaterialPropertyBlock();

        Clear();
    }

    void Tick() {
        if (!didInit)
            Init();

        Clear();
        Collect();
        Render_v2();
    }

    void Clear() {
        windows = new List<WindowNode>();

        depth = 0;

        textIndex = 0;
        imageIndex = 0;

        imagesColors.Clear();
        textColors.Clear();

        imagesUvs.Clear();
        textUvs.Clear();
    }

    void Collect() {
        for (int i = 0; i < 20; i++) {
            AddImage(new Vector3(i * 0.2f, 0, 0), Color.green);
            AddText('ß', new Vector3(i * 0.2f, 0, 0), Color.white);
        }

        for (int i = 0; i < 20; i++) {
            //AddText('ß', new Vector3(i * 0.2f, 0, 0), Color.white);
        }
    }

    void AddText(char ch, Vector3 position, Color color) {
        font.GetCharacterInfo(ch, out CharacterInfo charInfo);

        if (charInfo.flipped) {
            textMatrices[textIndex] = Matrix4x4.TRS(position.SetZ(depth * depthStep), Quaternion.Euler(0, 0, 90), Vector3.one * 0.2f);
        } else {
            textMatrices[textIndex] = Matrix4x4.TRS(position.SetZ(depth * depthStep), Quaternion.Euler(0, 0, 0), Vector3.one * 0.2f);
        }

        textColors.Add(color);
        textUvs.Add(new Vector4(charInfo.uvBottomLeft.x, charInfo.uvBottomLeft.y, charInfo.uvTopRight.x, charInfo.uvTopRight.y));

        textIndex++;
        depth++;
    }

    void AddImage(Vector3 position, Color color) {
        Sprite sprite = spritesDict["ui_button"];

        imageMatrices[imageIndex] = Matrix4x4.TRS(position.SetZ(depth * depthStep), Quaternion.Euler(0, 0, 0), Vector3.one);
        imagesColors.Add(color);
        imagesUvs.Add(new Vector4(
            sprite.rect.xMin / sprite.texture.width,
            sprite.rect.yMin / sprite.texture.height,
            sprite.rect.xMax / sprite.texture.width,
            sprite.rect.yMax / sprite.texture.height
        ));


        imageIndex++;
        depth++;
    }

    void Render_v2() {
        // Images
        imageParams.matProps.SetVectorArray("_Color", imagesColors);
        imageParams.matProps.SetVectorArray("_uv", imagesUvs);

        Sprite sprite = spritesDict["ui_button"];

        Mesh_Slice9 slice = new Mesh_Slice9();
        slice.Create();
        slice.SetSize(new Vector2(t1.x, t1.y), t0, new Vector4(
            sprite.border.x / sprite.rect.size.x * 2,
            sprite.border.y / sprite.rect.size.y * 2,
            sprite.border.z / sprite.rect.size.x * 2,
            sprite.border.w / sprite.rect.size.y * 2
        ));

        Graphics.DrawMeshInstanced(slice.mesh, 0, material_image, imageMatrices, imageIndex, imageParams.matProps, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0);

        // Text
        textParams.matProps.SetVectorArray("_Color", textColors);
        textParams.matProps.SetVectorArray("_uv", textUvs);

        Graphics.DrawMeshInstanced(meshFactory.quad, 0, material_text, textMatrices, textIndex, textParams.matProps, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0);
    }

    void Render() {
        Sprite sprite = spritesDict["ui_button"];
        //Debug.Log($"sprite uv: {string.Join(',',sprite.uv)} + {sprite.rect}");
        //Debug.Log($"sprite borders: {sprite.border}");

        Vector4 uv = new Vector4(
            sprite.rect.xMin / sprite.texture.width,
            sprite.rect.yMin / sprite.texture.height,
            sprite.rect.xMax / sprite.texture.width,
            sprite.rect.yMax / sprite.texture.height
        );

        Color c = Color.white * 0.8f;
        c.a = 1.0f;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetVector("_Color", c);
        block.SetVector("_uv", uv);

        RenderParams rp = new RenderParams();
        rp.matProps = block;
        rp.material = material_image;

        Mesh_Slice9 slice = new Mesh_Slice9();
        slice.Create();
        slice.SetSize(new Vector2(t1.x, t1.y), t0, new Vector4(
            sprite.border.x / sprite.rect.size.x * 2,
            sprite.border.y / sprite.rect.size.y * 2,
            sprite.border.z / sprite.rect.size.x * 2,
            sprite.border.w / sprite.rect.size.y * 2
        ));

        RenderParams fontRenderParams = new RenderParams(material_text);

        //Graphics.DrawMesh(slice.mesh, Matrix4x4.identity, material_image, 0, Camera.main, 0, block);
        for (int i = 0; i < 10; i++) {
            Vector3 position = new Vector3(0.25f * i, 0, i * 0.001f);

            Graphics.RenderMesh(rp, slice.mesh, 0, Matrix4x4.Translate(position));
            //RenderText('A', Color.red, position);
        }

        for (int i = 0; i < 10; i++) {
            Vector3 position = new Vector3(0.25f * i, 0, i * 0.001f);

            //Graphics.RenderMesh(rp, slice.mesh, 0, Matrix4x4.Translate(position));
            RenderText('A', Color.red, position);
        }

        //Graphics.RenderMesh()
    }

    void RenderText(char ch, Color color, Vector3 position) {
        font.GetCharacterInfo(ch, out CharacterInfo charInfo);

        RenderParams rp = new RenderParams(material_text);
        rp.matProps = new MaterialPropertyBlock();
        List<Vector4> colors = new List<Vector4>();
        List<Vector4> uvs = new List<Vector4>();


        Matrix4x4 mat = Matrix4x4.identity;

        for (int i = 0; i < 1; i++) {
            if (charInfo.flipped) {
                mat = Matrix4x4.TRS(position, Quaternion.Euler(0, 0, 90), Vector3.one * 0.2f);
            } else {
                mat = Matrix4x4.TRS(position, Quaternion.Euler(0,0,0), Vector3.one * 0.2f);
            }

            colors.Add(color);
            uvs.Add(new Vector4(charInfo.uvBottomLeft.x, charInfo.uvBottomLeft.y, charInfo.uvTopRight.x, charInfo.uvTopRight.y));
        }

        rp.matProps.SetVectorArray("_Color", colors);
        rp.matProps.SetVectorArray("_uv", uvs);

        Graphics.RenderMesh(rp, meshFactory.quad, 0, mat);
    }

    void RenderButton() {

    }

    public static void Window(Rect rect) {

    }

    public static bool Button(string text) {
        return false;
    }

    class Node {
        Rect rect;
    }

    class ButtonNode : Node {

    }

    class WindowNode : Node {

    }

}


public static class BazaUIExt {

    public static Vector3 SetZ(this Vector3 v, float z) {
        return new Vector3(v.x, v.y, z);
    }

}