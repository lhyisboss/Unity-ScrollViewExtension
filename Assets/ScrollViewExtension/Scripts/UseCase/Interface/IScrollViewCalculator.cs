using System;
using UnityEngine;

namespace ScrollViewExtension.Scripts.UseCase.Interface
{
    /// <summary>
    /// スクロールビューの計算を行うインターフェース
    /// </summary>
    public interface IScrollViewCalculator : IDisposable
    {
        /// <summary>
        /// インスタンスの数を計算
        /// </summary>
        /// <returns>インスタンス数</returns>
        int CalculateInstanceNumber();

        /// <summary>
        /// コンテンツのサイズを計算
        /// </summary>
        /// <returns>コンテンツのサイズ</returns>
        Vector2 CalculateContentSize();

        /// <summary>
        /// バーの位置を計算
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns>バーの位置</returns>
        float CalculateBarPosition(int index);

        /// <summary>
        /// インデックス、カウント、コンテンツの位置に基づくオフセットを計算
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <param name="count">カウント</param>
        /// <param name="contentPos">コンテンツの位置</param>
        /// <returns>オフセット</returns>
        Vector4 CalculateOffset(int index, int count, Vector3 contentPos);

        /// <summary>
        /// 0:スクロールしない、正数:下方向、負数:上方向でスクロールを計算
        /// </summary>
        /// <param name="currentPadding">現在のパディング</param>
        /// <param name="newPadding">新しいパディング</param>
        /// <param name="startIndex">スタートのインデックス</param>
        /// <returns>スクロール方向</returns>
        int CalculateRolling(Vector4 currentPadding, Vector4 newPadding, int startIndex);
    }
}