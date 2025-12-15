using System.Collections;
using UnityEngine;

namespace _Project.Features.Battle.Scripts
{
    public class SpriteView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private Material flashMaterial;
        [SerializeField] private float duration = 0.5f;
        private float m_NowDuration;

        private Material m_OriginMaterial;
        private Coroutine m_FlashRoutine;
        private static readonly int FlashPercent = Shader.PropertyToID("FlashPercent");

        private void Awake()
        {
            // SpriteRenderer 참조가 비어 있으면 자기 자신에서 찾아온다.
            if (m_SpriteRenderer == null)
            {
                m_SpriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void Start()
        {
            if (m_SpriteRenderer != null)
            {
                m_OriginMaterial = m_SpriteRenderer.material;
            }
        }

        /// <summary>
        /// 외부에서 SpriteRenderer를 교체하고 싶을 때 사용.
        /// (예: 자식 오브젝트의 스프라이트를 관리할 때)
        /// </summary>
        public void SetSpriteRenderer(SpriteRenderer renderer)
        {
            m_SpriteRenderer = renderer;
            if (m_SpriteRenderer != null)
            {
                m_OriginMaterial = m_SpriteRenderer.material;
            }
        }

        /// <summary>
        /// 오른쪽을 보고 있는지 여부에 따라 flipX 설정.
        /// 프로젝트 기준에 맞게 true/false 방향만 맞추면 된다.
        /// </summary>
        /// <param name="isLookingRight">true면 오른쪽을 보고 있는 상태.</param>
        public void SetLookDirection(bool isLookingRight)
        {
            if (m_SpriteRenderer == null)
                return;

            // 스프라이트 기본 방향이 오른쪽이라고 가정.
            // 필요하면 !isLookingRight 로 반전해서 쓰면 됨.
            m_SpriteRenderer.flipX = !isLookingRight;
        }

        /// <summary>
        /// flipX를 직접 제어하고 싶을 때 사용하는 단순 버전.
        /// </summary>
        public void SetFlipX(bool flipX)
        {
            if (m_SpriteRenderer == null)
                return;

            m_SpriteRenderer.flipX = flipX;
        }

        public void Flash()
        {
            if (m_SpriteRenderer == null)
                return;

            if (m_FlashRoutine != null)
            {
                StopCoroutine(m_FlashRoutine);
            }

            m_FlashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            m_NowDuration = 0;
            m_SpriteRenderer.material = flashMaterial;
            while (m_NowDuration <= duration)
            {
                // m_SpriteRenderer.material.SetFloat(FlashPercent, 1 - m_NowDuration / duration);
                m_NowDuration += Time.deltaTime;
                yield return null;
            }
            m_SpriteRenderer.material = m_OriginMaterial;
            m_FlashRoutine = null;
        }
    }
}