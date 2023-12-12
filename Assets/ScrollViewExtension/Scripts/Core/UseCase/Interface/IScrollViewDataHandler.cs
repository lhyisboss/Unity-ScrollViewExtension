using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Core.UseCase.Interface
{
    /// <summary>
    /// スクロールビューのデータハンドラのインターフェースです。
    /// </summary>
    /// <typeparam name="TData">データの型。</typeparam>
    internal interface IScrollViewDataHandler<TData> : IDisposable
    {
        /// <summary>
        /// アイテムを作成します。
        /// </summary>
        /// <param name="size">アイテムのサイズ。</param>
        /// <returns>作成されたアイテム。</returns>
        TData CreateItem(Vector2 size);

        /// <summary>
        /// 指定された範囲のアイテムを取得します。
        /// </summary>
        /// <param name="index">開始インデックス。</param>
        /// <param name="count">取得するアイテムの数。</param>
        /// <returns>取得されたアイテムのリスト。</returns>
        List<TData> GetRange(int index, int count);

        /// <summary>
        /// 指定されたコンテンツ位置のアイテムを取得します。
        /// </summary>
        /// <param name="contentPos">コンテンツの位置。</param>
        /// <param name="count">取得するアイテムの数。</param>
        /// <returns>取得されたアイテムのリスト。</returns>
        List<TData> GetRange(Vector3 contentPos, int count);

        /// <summary>
        /// 指定したインデックスからの位置情報を更新します。
        /// </summary>
        /// <param name="index">開始するインデックス。</param>
        void UpdatePositionsFromIndex(int index);

        void SetScrollItems(List<TData> data);
    }
}