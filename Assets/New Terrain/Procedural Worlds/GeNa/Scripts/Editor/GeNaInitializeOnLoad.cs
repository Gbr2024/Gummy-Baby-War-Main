using System.Collections.Generic;
using GeNa.Core;
using UnityEngine;
using UnityEditor;
[InitializeOnLoad]
public class GeNaInitializeOnLoad
{
    private static Constants.RenderPipeline previousRenderPipeline;
    private static float editorDeltaTime = 0f;
    private static float lastTimeSinceStartup = 0f;
    private static float elapsedTime = 0f;
    private static bool inAnimationMode = false;
    private static List<GeNaAnimatorDecorator> m_animators = new List<GeNaAnimatorDecorator>();
    private static List<GeNaParticleDecorator> m_particles = new List<GeNaParticleDecorator>();
    static GeNaInitializeOnLoad()
    {
        EditorApplication.update -= Update;
        EditorApplication.update += Update;
        EditorApplication.quitting -= OnEditorQuit;
        EditorApplication.quitting += OnEditorQuit;
        AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReloads;
        AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReloads;
        AssemblyReloadEvents.afterAssemblyReload -= OnBeforeAssemblyReloads;
        AssemblyReloadEvents.afterAssemblyReload += OnBeforeAssemblyReloads;
        GeNaEvents.onDecoratorSpawned -= DecoratorSpawned;
        GeNaEvents.onDecoratorSpawned += DecoratorSpawned;
        InAnimationMode(false);
        previousRenderPipeline = GeNaUtility.GetActivePipeline();
    }
    private static void DecoratorSpawned(IDecorator decorator)
    {
        switch (decorator)
        {
            case GeNaAnimatorDecorator animatorDecorator:
                m_animators.Add(animatorDecorator);
                break;
            case GeNaParticleDecorator particleDecorator:
                m_particles.Add(particleDecorator);
                break;
        }
    }
    private static void OnEditorQuit()
    {
        GeNaFactory.Dispose();
    }
    private static void OnBeforeAssemblyReloads()
    {
        GeNaFactory.Dispose();
    }
    private static void OnAfterAssemblyReloads()
    {
        GeNaUtility.UpdateSplinesRenderPipeline(previousRenderPipeline, out previousRenderPipeline);
    }
    private static void SetEditorDeltaTime()
    {
        if (lastTimeSinceStartup == 0f)
        {
            lastTimeSinceStartup = (float)EditorApplication.timeSinceStartup;
        }
        editorDeltaTime = (float)EditorApplication.timeSinceStartup - lastTimeSinceStartup;
        lastTimeSinceStartup = (float)EditorApplication.timeSinceStartup;
        elapsedTime += editorDeltaTime;
    }
    public static void InAnimationMode(bool value)
    {
        if (value == inAnimationMode)
            return;
        inAnimationMode = value;
        if (inAnimationMode)
            AnimationMode.StartAnimationMode();
        else
            AnimationMode.StopAnimationMode();
    }
    private static void RunAnimations()
    {
        bool decoratorsEmpty = m_animators.Count == 0;
        if (decoratorsEmpty)
            return;
        bool repaint = false;
        bool isPlaying = false;
        List<GeNaAnimatorDecorator> animatorsToDelete = new List<GeNaAnimatorDecorator>();
        foreach (GeNaAnimatorDecorator decorator in m_animators)
        {
            if (decorator == null)
                continue;
            if (!decorator.isActiveAndEnabled)
                continue;
            Animator[] animators = decorator.Animators;
            if (animators == null)
                continue;
            bool finished = true;
            foreach (Animator animator in animators)
            {
                if (!animator.enabled)
                    continue;
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                float normalizedTime = stateInfo.normalizedTime;
                // var loop = stateInfo.loop;
                if (normalizedTime >= 1f)
                {
                    decorator.enabled = false;
                    continue;
                }
                finished = false;
                repaint = true;
                isPlaying = true;
                animator.Update(editorDeltaTime);
            }
            if (finished)
                animatorsToDelete.Add(decorator);
        }
        m_animators.RemoveAll(animator => animator == null || animatorsToDelete.Contains(animator));
        if (repaint)
            SceneView.RepaintAll();
        InAnimationMode(isPlaying);
    }
    private static void RunParticles()
    {
        bool decoratorsEmpty = m_particles.Count == 0;
        if (decoratorsEmpty)
            return;
        bool repaint = false;
        List<GeNaParticleDecorator> particlesToDelete = new List<GeNaParticleDecorator>();
        foreach (GeNaParticleDecorator decorator in m_particles)
        {
            if (decorator == null)
                continue;
            if (!decorator.enabled)
                continue;
            ParticleSystem[] particles = decorator.Particles;
            if (particles == null)
                continue;
            bool finished = true;
            foreach (ParticleSystem particle in particles)
            {
                ParticleSystem.MainModule main = particle.main;
                decorator.Time += editorDeltaTime;
                if (decorator.Time >= main.duration)
                {
                    decorator.enabled = false;
                    continue;
                }
                finished = false;
                repaint = true;
                particle.Stop();
                particle.Clear();
                particle.randomSeed = 5;
                particle.Simulate(decorator.Time, true, true);
            }
            if (finished)
                particlesToDelete.Add(decorator);
        }
        m_particles.RemoveAll(particle => particle == null || particlesToDelete.Contains(particle));
        if (repaint)
            SceneView.RepaintAll();
    }
    private static void Update()
    {
        SetEditorDeltaTime();
        RunAnimations();
        RunParticles();
    }
}