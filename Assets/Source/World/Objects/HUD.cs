using Assets.Source.Models;
using Assets.Source.Systems;
using Assets.Source.Systems.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Source.World.Objects
{
    public class HUD : InGameObject
    {
        private Camera _camera;
        private List<GameObject> _objects = new();

        public HUD(Camera camera)
        {
            _camera = camera;
        }

        protected override void Build(GameObject obj)
        {
            obj.AddComponent<Canvas>();
            obj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            obj.GetComponent<Canvas>().worldCamera = _camera;
            obj.GetComponent<Canvas>().planeDistance = 1;
        }

        protected override (Vector3 position, Vector3 velocity) CalculateMovement() => (Position, Velocity);

        protected override void OnStart()
        {
            var crosshair = new GameObject();
            crosshair.name = "Crosshair";
            var spriteSheet = TextureHelper.Textures[TextureSheet.HUD];
            crosshair.AddComponent<MeshRenderer>();
            crosshair.AddComponent<MeshFilter>();

            float scale = 50f;
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0.5f, 0.5f, 0) * scale, new Vector3(0.5f, -0.5f, 0) * scale, new Vector3(-0.5f, 0.5f, 0) * scale,
                new Vector3(-0.5f, -0.5f, 0) * scale, new Vector3(-0.5f, 0.5f, 0) * scale, new Vector3(0.5f, -0.5f, 0) * scale
            };
            int[] triangles = new int[] { 0, 1, 2, 3, 4, 5 };
            Vector2[] uv = TextureHelper.GetVoxelUV(spriteSheet.Width, spriteSheet.Height, spriteSheet.SpriteSize, new Vector2(0, 0), 0.003f);
            var mesh = new Mesh()
            {
                vertices = vertices,
                triangles = triangles,
                uv = uv
            };

            crosshair.GetComponent<MeshFilter>().mesh = mesh;
            crosshair.GetComponent<Renderer>().material.SetTexture("_MainTex", spriteSheet.Texture2D);
            crosshair.GetComponent<Renderer>().material.ToFadeMode();
            crosshair.GetComponent<Renderer>().receiveShadows = false;
            crosshair.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
            crosshair.transform.SetParent(GameObject.transform);
            crosshair.transform.localPosition = Vector3.zero;
            crosshair.transform.localScale = Vector3.one;
            crosshair.transform.localPosition -= new Vector3((scale / spriteSheet.SpriteSize) / 2, (scale / spriteSheet.SpriteSize) / 2, 0);
            _objects.Add(crosshair);
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnObjectDispose()
        {
            foreach (var obj in _objects)
                GameObject.Destroy(obj);
        }
    }
}
