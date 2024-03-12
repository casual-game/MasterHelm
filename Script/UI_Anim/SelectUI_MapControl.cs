using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public partial class SelectUI : MonoBehaviour
{
   [FoldoutGroup("MapControl")] public List<StageBanner> banners = new List<StageBanner>();
   [FoldoutGroup("MapControl")] public AudioSource windSource;
   [PropertyRange(0, "StageMax")] public int currentStage = 0,selectedStage = 0;
   [FoldoutGroup("MapControl")] public List<Transform> farObjects = new List<Transform>();
   [FoldoutGroup("MapControl")] public float farObjectCheckDist = 25.0f,farObjectMoveDist = 5.0f;
   [FoldoutGroup("MapControl")] public AnimationCurve farMoveCurve;
   [FoldoutGroup("MapControl")] public ParticleImage currentStageParticle;
   
   private Dictionary<Transform, Vector3> farObjectsPos = new Dictionary<Transform, Vector3>();
   private float screenRatio;
   private Transform camT;
   private Vector2 moveVec = Vector2.zero;
   private bool _isControlling = false;
   private BoxCollider controllableArea;
   private bool canDrag = true;

   private int StageMax()
   {
      return banners.Count-1;
   }
   private void Setting_MapControl()
   {
      screenRatio = (Screen.width*1.0f) / (Screen.height*1.0f);
      camT = cams[0].transform.parent;
      controllableArea = GetComponent<BoxCollider>();
   
      foreach (var farObject in farObjects) farObjectsPos.Add(farObject,farObject.position);
      foreach (var banner in banners) banner.Setting();
      UpdateStage();
   }
   public void E_PointerDown(PointerEventData data)
   {
      if (!canDrag) return;
      moveVec = data.delta;
      _isControlling = true;
      MoveMap();
   }
   public void E_PointerUp(PointerEventData data)
   {
      if (!canDrag) return;
      moveVec = data.delta;
      _isControlling = false;
      MoveMap();
   }
   public void E_Drag(PointerEventData data)
   {
      if (!canDrag) return;
      moveVec = data.delta;
      MoveMap();
   }
   public void E_PointerClick(PointerEventData data)
   {
      if (!canDrag) return;
      if((data.pressPosition - data.position).sqrMagnitude > 400) return;
      if (_stageStart)
      {
         StageStart_Exit();
         return;
      }

      if (_seqStageStart.isAlive) return;
      float mapHeight = cams[0].orthographicSize*2;
      float mapWidth = mapHeight * screenRatio;
      float inputHeightRatio = data.position.y / (Screen.height * 1.0f);
      float inputWidthRatio = data.position.x / (Screen.width * 1.0f);

      Vector3 camTPos = camT.position;
      float worldXBegin = camTPos[0] - cams[0].orthographicSize * screenRatio;
      float worldXFin = camTPos[0] + cams[0].orthographicSize * screenRatio;
      float worldYBegin = camTPos[2] - cams[0].orthographicSize;
      float worldYFin = camTPos[2] + cams[0].orthographicSize;
      float worldHeight = cams[0].orthographicSize * 2;
      float worldWidth = worldHeight * screenRatio;
      StageBanner b = null;
      float dist = Mathf.Infinity;
      Vector2 screenRatioDist = Vector2.zero;
      foreach (var banner in banners)
      {
         Vector3 pos = banner.transform.position;
         float bannerWidthRatio = (pos.x - worldXBegin) / worldWidth;
         if(0>bannerWidthRatio || bannerWidthRatio>1) continue;
         float bannerHeightRatio = (pos.z - worldYBegin) / worldHeight;
         if(0>bannerHeightRatio || bannerHeightRatio>1) continue;
         Vector2 vec = new Vector2(bannerWidthRatio - inputWidthRatio, bannerHeightRatio - inputHeightRatio);
         float ratioDist = vec.magnitude;
         if (dist > ratioDist)
         {
            dist = ratioDist;
            b = banner;
            screenRatioDist = vec;
         }
      }

      bool widthCollided = (0.75f / mapWidth) > Mathf.Abs(screenRatioDist.x);
      bool heightCollided = (1.5f / mapHeight) > Mathf.Abs(screenRatioDist.y);
      if (b != null && widthCollided && heightCollided)
      {
         SelectStage(b);
         if(b.activated) StageStart_Enter(b);
         else
         {
            tip.Tip_RequireClear();
            StageStart_JustMove(b);
         }
      }
   }
   public void Update()
   {
      if (!_isControlling)
      {
         moveVec = Vector2.Lerp(moveVec,Vector2.zero,15*Time.deltaTime);
         MoveMap();
      }
   }

   public void MoveMap(bool force = false)
   {
      if (_stageStart&&!force) return;
      float mapHeight = cams[0].orthographicSize*2;
      float mapWidth = mapHeight * screenRatio;
      float inputHeightRatio = moveVec.y / (Screen.height * 1.0f);
      float inputWidthRatio = moveVec.x / (Screen.width * 1.0f);
      Vector3 finalMoveVec = new Vector3(-mapWidth * inputWidthRatio, 0, -mapHeight * inputHeightRatio);
      Vector3 pos = camT.position + finalMoveVec;
      Vector3 center = transform.position+controllableArea.center,size = controllableArea.size*0.5f;
      pos.x = Mathf.Clamp(pos.x, center.x-size.x+mapWidth*0.5f, center.x+size.x-mapWidth*0.5f);
      pos.z = Mathf.Clamp(pos.z, center.z-size.z+mapHeight*0.5f, center.z+size.z-mapHeight*0.5f);
      camT.position = pos;
      foreach (var farObject in farObjects)
      {
         Vector3 farObjectPos = farObjectsPos[farObject];
         Vector3 distVec = farObjectPos - camT.position;
         float dist = distVec.magnitude;
         Vector3 foPos;
         if (dist > farObjectCheckDist)
         {
            foPos = farObjectPos + distVec.normalized * farObjectMoveDist;
         }
         else
         {
            float farRatio = farMoveCurve.Evaluate(dist / farObjectCheckDist);
            foPos = farObjectPos + distVec.normalized * (farRatio * farObjectMoveDist);
         }
         foPos.y = farObjectPos.y;
         farObject.position = foPos;
      }

      float speed = Mathf.Clamp(finalMoveVec.magnitude / Time.deltaTime,0,10);
      windSource.volume = Mathf.Lerp(windSource.volume, speed*0.075f, Time.deltaTime * 8.5f);
   }

   public void MoveMap(Vector3 pos)
   {
      float mapHeight = cams[0].orthographicSize*2;
      float mapWidth = mapHeight * screenRatio;
      Vector3 center = transform.position+controllableArea.center,size = controllableArea.size*0.5f;
      pos.x = Mathf.Clamp(pos.x, center.x-size.x+mapWidth*0.5f, center.x+size.x-mapWidth*0.5f);
      pos.z = Mathf.Clamp(pos.z, center.z-size.z+mapHeight*0.5f, center.z+size.z-mapHeight*0.5f);
      camT.position = pos;

      foreach (var farObject in farObjects)
      {
         Vector3 farObjectPos = farObjectsPos[farObject];
         Vector3 distVec = farObjectPos - camT.position;
         float dist = distVec.magnitude;
         Vector3 foPos;
         if (dist > farObjectCheckDist)
         {
            foPos = farObjectPos + distVec.normalized * farObjectMoveDist;
         }
         else
         {
            float farRatio = farMoveCurve.Evaluate(dist / farObjectCheckDist);
            foPos = farObjectPos + distVec.normalized * (farRatio * farObjectMoveDist);
         }
         foPos.y = farObjectPos.y;
         farObject.position = foPos;
      }
   }
   public void UpdateStage()
   {
      for(int i =0; i<banners.Count; i++)
      {
         var banner = banners[i];
         banner.activated = i <= currentStage;
         banner.UpdateBanner();
         if(i == selectedStage) SelectStage(banner);
      }
   }
   public void SelectStage(StageBanner b)
   {
      for(int i =0; i<banners.Count; i++)
      {
         var banner = banners[i];
         if (banner == b)
         {
            banner.selected = true;
            currentStageParticle.transform.position = banner.transform.position;
            if(!currentStageParticle.isPlaying && b.activated) currentStageParticle.Play();
            else if(!b.activated) currentStageParticle.Stop(true);
            selectedStage = i;
         }
         else banner.selected = false;
         banner.UpdateBanner();
      }
   }

   public Vector3 ClampVec(Vector3 vec)
   {
      float mapHeight = cams[0].orthographicSize*2;
      float mapWidth = mapHeight * screenRatio;
      Vector3 center = transform.position+controllableArea.center,size = controllableArea.size*0.5f;
      vec.x = Mathf.Clamp(vec.x, center.x-size.x+mapWidth*0.5f, center.x+size.x-mapWidth*0.5f);
      vec.z = Mathf.Clamp(vec.z, center.z-size.z+mapHeight*0.5f, center.z+size.z-mapHeight*0.5f);
      return vec;
   }
}
