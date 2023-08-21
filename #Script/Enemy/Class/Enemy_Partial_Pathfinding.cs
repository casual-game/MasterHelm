using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Enemy : MonoBehaviour
{
    private void PF_Setting()
    {
        paths.Clear();
    }

    private void PF_Disable()
    {
        PF_StopPath();
    }
    
    private Coroutine currentPathfinding=null;
    public List<Path> paths = new List<Path>();
    private Seeker seeker;
    private Path pf_path;
    //메인 함수
    public Vector3 PF_GetPosition(int currentIndex)
    {
        return paths[0].vectorPath[currentIndex];
    }
    public Vector3 PF_GetDestination()
    {
        return paths[0].vectorPath[paths[0].vectorPath.Count-1];
    }
    public int PF_Count()
    {
        return paths.Count;
    }
    public void PF_StopPath()
    {
        if (currentPathfinding== null) return;
        StopCoroutine(currentPathfinding);
        currentPathfinding = null;
    }
    //메인 CPF
    public IEnumerator CPF_StartPath(float delay,Transform from,Transform to,float dist)
    {
        PF_StopPath();
        //일단 path 하나 생성
        if(!seeker.IsDone()) seeker.CancelCurrentPathRequest();
        Vector3 targetVec = to.position + (from.position-to.position).normalized*dist;
        Path path = seeker.StartPath(transform.position, targetVec);
        yield return StartCoroutine (path.WaitForPath());
        paths.Add(path);
        //path 생성 루프 실행
        currentPathfinding = StartCoroutine(cpf_startpath(delay,from,to,dist));
    }
    public IEnumerator CPF_RandomPath(float distance)
    {
        PF_StopPath();
        currentPathfinding = StartCoroutine(cpf_randompath(distance));
        yield return currentPathfinding;
    }
    public IEnumerator CPF_FleePath(float distance)
    {
        PF_StopPath();
        if (!seeker.IsDone())
        {
            seeker.CancelCurrentPathRequest();
        }
        FleePath rpath = FleePath.Construct(transform.position,Player.instance.transform.position,Mathf.RoundToInt(195 * distance));
        rpath.spread = 5000;
        Path path = seeker.StartPath(rpath);
        yield return StartCoroutine(path.WaitForPath());
        paths.Add(path);
        currentPathfinding = null;
    }
    //Animator에서 호출
    public void PF_UpdatePath(ref int currentIndex,ref bool arrived)
    {
        float closeStandard = 0.25f;
        //Path가 없으면 초기화.
        if (paths.Count == 0)
        {
            currentIndex = 0;
            return;
        }
        //Path를 최신 버전으로 갱신
        while (paths.Count > 1)
        {
            paths.RemoveAt(0);
            currentIndex = 0;
        }
        
        Vector3 pathVec = paths[0].vectorPath[currentIndex];
        //Path의 끝에 도착하지 않은 경우
        if (currentIndex < paths[0].vectorPath.Count - 1)
        {
            //이번 pathVec과 가까운 경우
            if (Vector3.SqrMagnitude(pathVec - transform.position) < closeStandard)
            {
                currentIndex += 1;
            }
            //아직 pathVec과 멀리 있는 경우는 그대로.
            arrived = false;
        }
        else if(Vector3.SqrMagnitude(pathVec - transform.position) < closeStandard)
        {
            arrived = true;
        }
    }
    //CPF(서브)
    private IEnumerator cpf_startpath(float delay,Transform from,Transform to,float dist)
    {
        yield return null;
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if(!seeker.IsDone()) seeker.CancelCurrentPathRequest();
            Vector3 targetVec = to.position + (from.position-to.position).normalized*dist;
            Path path = seeker.StartPath(transform.position, targetVec);
            yield return StartCoroutine (path.WaitForPath());
            paths.Add(path);
        }
    }
    private IEnumerator cpf_randompath(float distance)
    {
        yield return null;

        if (!seeker.IsDone())
        {
            seeker.CancelCurrentPathRequest();
        }

        RandomPath rpath = RandomPath.Construct(transform.position, Mathf.RoundToInt(195 * distance));
        rpath.spread = 5000;
        Path path = seeker.StartPath(rpath);
        yield return StartCoroutine(path.WaitForPath());
        paths.Add(path);
        currentPathfinding = null;
    }
    
}
