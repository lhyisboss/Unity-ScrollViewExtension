using System;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Entity.Interface
{
    /// <summary>
    /// グループレイアウト实体を表すインターフェースです。
    /// IDisposableインターフェースを継承します。
    /// </summary>
    public interface IGroupLayoutEntity : IDisposable
    {
        /// <summary>
        /// 配置が逆であるかどうかを取得または設定します。
        /// </summary>
        /// <returns>Trueなら配置は逆、falseなら通常の配置。</returns>
        bool ReverseArrangement { get; set; }

        /// <summary>
        /// 子が強制的に幅を拡大するかどうかを取得または設定します。
        /// </summary>
        /// <returns>Trueなら子は強制的に幅を拡大、falseなら子は強制的に幅を拡大しません。</returns>
        bool ChildForceExpandWidth { get; set; }

        /// <summary>
        /// 子が強制的に高さを拡大するかどうかを取得または設定します。
        /// </summary>
        /// <returns>Trueなら子は強制的に高さを拡大、falseなら子は強制的に高さを拡大しません。</returns>
        bool ChildForceExpandHeight { get; set; }

        /// <summary>
        /// 子が幅を制御するかどうかを取得または設定します。
        /// </summary>
        /// <returns>Trueなら子は幅を制御、falseなら子は幅を制御しません。</returns>
        bool ChildControlWidth { get; set; }

        /// <summary>
        /// 子が高さを制御するかどうかを取得または設定します。
        /// </summary>
        /// <returns>Trueなら子は高さを制御、falseなら子は高さを制御しません。</returns>
        bool ChildControlHeight { get; set; }

        /// <summary>
        /// 子のアラインメントを取得または設定します。
        /// </summary>
        /// <returns>子のアラインメント。</returns>
        TextAnchor ChildAlignment { get; set; }

        /// <summary>
        /// 子が幅を拡大するかどうかを取得または設定します。
        /// </summary>
        /// <returns>Trueなら子は幅を拡大、falseなら子は幅を拡大しません。</returns>
        bool ChildScaleWidth { get; set; }

        /// <summary>
        /// 子が高さを拡大するかどうかを取得または設定します。
        /// </summary>
        /// <returns>Trueなら子は高さを拡大、falseなら子は高さを拡大しません。</returns>
        bool ChildScaleHeight { get; set; }

        /// <summary>
        /// 最小アンカーを取得または設定します。
        /// </summary>
        /// <returns>最小アンカー。</returns>
        Vector2 MinAnchor { get; set; }

        /// <summary>
        /// 最大アンカーを取得または設定します。
        /// </summary>
        /// <returns>最大アンカー。</returns>
        Vector2 MaxAnchor { get; set; }
    }
}