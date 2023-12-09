using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Entity.Interface
{
    public interface IScrollViewEntity<TData> : IDisposable where TData : new()
    {
        /// <summary>
        /// 現在のビューの長さを取得します。
        /// </summary>
        float GetViewLength { get; }

        /// <summary>
        /// ビューの長さを設定します。
        /// </summary>
        Vector2 SetViewLength { set; }

        /// <summary>
        /// 現在の全データを取得します。
        /// </summary>
        List<TData> Data { get; set; }

        /// <summary>
        /// コンテンツサイズを取得または設定します。
        /// </summary>
        Vector2 ContentSize { get; set; }

        /// <summary>
        /// アイテムの位置を計算します。
        /// </summary>
        /// <param name="index">アイテムのインデックス</param>
        /// <returns>アイテムの位置</returns>
        Vector2 CalculateItemPosition(int index);

        void UpdateItemPosition(int index, Vector2 newPos);

        /// <summary>
        /// DefaultPaddingの値を取得または設定します。
        /// </summary>
        RectOffset DefaultPadding { get; set; }

        /// <summary>
        /// LayoutGroupのSpacingを取得または設定します。
        /// </summary>
        float Spacing { get; set; }

        /// <summary>
        /// アイテムを作成します。
        /// </summary>
        /// <param name="size">アイテムのサイズ</param>
        /// <param name="position">アイテムの位置</param>
        /// <returns>作成したアイテム</returns>
        TData CreateItem(Vector2 size, Vector2 position);
        
        /// <summary>
        /// 指定された範囲のアイテムを取得します。
        /// </summary>
        /// <param name="start">開始インデックス</param>
        /// <param name="count">取得するアイテムの数</param>
        /// <returns>アイテムのリスト</returns>
        List<TData> GetRange(int start, int count);

        /// <summary>
        /// アイテムの最小の長さを取得します。
        /// </summary>
        /// <returns>アイテムの最小の長さ</returns>
        float GetItemMinLength();

        /// <summary>
        /// コンテンツの長さを取得します。
        /// </summary>
        /// <param name="count">アイテムの数</param>
        /// <param name="index">開始インデックス（デフォルトは0）</param>
        /// <returns>コンテンツの長さ</returns>
        float GetContentLength(int count, int index = 0);

        /// <summary>
        /// アイテムのサイズを取得します。
        /// </summary>
        /// <param name="index">アイテムのインデックス</param>
        /// <returns>アイテムのサイズ</returns>
        float GetItemSize(int index);

        /// <summary>
        /// 垂直方向かどうかを示す値を取得します。
        /// </summary>
        /// <returns>垂直方向場合はtrue、そうでない場合はfalse。</returns>
        bool IsVertical();
    }
}