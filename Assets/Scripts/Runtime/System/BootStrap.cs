using SymphonyFrameWork.System;
using UnityEngine;

namespace SengokuNinjaVillage.System
{
    /// <summary>
    /// �Q�[���̏��������s��
    /// </summary>
    public static class BootStrap
    {
        /// <summary>
        /// �V�[�����[�h�O�̏�����
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            //�V�X�e���V�[�������[�h����
            var task = SceneLoader.LoadScene(SceneListEnum.System.ToString());
        }
    }
}
