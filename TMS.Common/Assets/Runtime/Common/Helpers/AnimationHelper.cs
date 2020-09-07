#region Usings

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace TMS.Common.Helpers
{
	/// <summary>
	/// Animation Helper
	/// </summary>
	public class AnimationHelper
	{
		/// <summary>
		/// The scripted animation name
		/// </summary>
		public const string ScriptedAnimationName = "Scripted Animation";

		private readonly IDictionary<string, AnimationCurve> _curves = new Dictionary<string, AnimationCurve>();
		private readonly IDictionary<string, Type> _curveSets = new Dictionary<string, Type>();
		private AnimationClip _clip;
		private GameObject _target;
		private WrapMode _wrapModeSelection;

		/// <summary>
		/// Creates the animation helper.
		/// </summary>
		/// <returns></returns>
		public static AnimationHelper CreateAnimationHelper()
		{
			return new AnimationHelper();
		}

		/// <summary>
		/// Begins the animation pipeline.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="wrapMode">The wrap mode.</param>
		/// <returns></returns>
		public AnimationHelper BeginAnimationPipeline(GameObject target, WrapMode wrapMode)
		{
			_target = target;
			SetCurveNames();

			// if animation component does not exist, add one
			if (!target.GetComponent<Animation>())
			{
				target.AddComponent<Animation>();
			}

			_wrapModeSelection = wrapMode;
			_clip = new AnimationClip();
			return this;
		}

		/// <summary>
		/// Ends the animation pipeline.
		/// </summary>
		/// <returns></returns>
		public AnimationHelper EndAnimationPipeline()
		{
			// set the curves 
			foreach (var curve in _curves)
			{
				Type type;
				_curveSets.TryGetValue(curve.Key, out type);
				_clip.SetCurve("curve_" + curve.Key, type, curve.Key, curve.Value);
			}

			// set the clip
			_clip.wrapMode = _wrapModeSelection;

			_clip.name = ScriptedAnimationName;

			var anim = _target.GetComponent<Animation>();
			anim.playAutomatically = false;
			anim.AddClip(_clip, _clip.name);
			anim.clip = _clip;

			return this;
		}

		/// <summary>
		/// Plays the anim.
		/// </summary>
		public void PlayAnim()
		{
			var anim = _target.GetComponent<Animation>();
			anim.Play();
		}

		/// <summary>
		/// Sets the curve names.
		/// </summary>
		private void SetCurveNames()
		{
			// position
			AddKeyWithCheck("localPosition.x", typeof (Transform));
			AddKeyWithCheck("localPosition.y", typeof (Transform));
			AddKeyWithCheck("localPosition.z", typeof (Transform));
			// scale
			AddKeyWithCheck("localScale.x", typeof (Transform));
			AddKeyWithCheck("localScale.y", typeof (Transform));
			AddKeyWithCheck("localScale.z", typeof (Transform));
			// rotation
			AddKeyWithCheck("localRotation.x", typeof (Transform));
			AddKeyWithCheck("localRotation.y", typeof (Transform));
			AddKeyWithCheck("localRotation.z", typeof (Transform));
			AddKeyWithCheck("localRotation.w", typeof (Transform));
			// color
			AddKeyWithCheck("_Color.r", typeof (Material));
			AddKeyWithCheck("_Color.g", typeof (Material));
			AddKeyWithCheck("_Color.b", typeof (Material));
			AddKeyWithCheck("_Color.a", typeof (Material));
			// orthographic size
			AddKeyWithCheck("orthographic size", typeof (Camera));
		}

		/// <summary>
		/// Adds the key with check.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="type">The type.</param>
		private void AddKeyWithCheck(string key, Type type)
		{
			if (!_curveSets.ContainsKey(key))
			{
				_curveSets.Add(key, type);
			}
		}

		/// <summary>
		/// Determines whether [is first frame] [the specified curve].
		/// </summary>
		/// <param name="curve">The curve.</param>
		/// <returns></returns>
		private static bool IsFirstFrame(AnimationCurve curve)
		{
			return curve.keys.Length == 0;
		}

		private static float GetTime(AnimationCurve curve)
		{
			var time = curve.keys[curve.keys.Length - 1].time;
			return time;
		}

		private static float GetFrom(AnimationCurve curve)
		{
			var from = curve.keys[curve.keys.Length - 1].value;
			return from;
		}

		private static float GetInTangent(AnimationCurve curve)
		{
			var inTangent = curve.keys[curve.keys.Length - 1].inTangent;
			return inTangent;
		}

		private static float GetOutTangent(AnimationCurve curve)
		{
			var outTangent = curve.keys[curve.keys.Length - 1].outTangent;
			return outTangent;
		}

		/// <summary>
		/// Adds the position animation.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="time">The time.</param>
		/// <param name="inTangent">The in tangent.</param>
		/// <param name="outTangent">The out tangent.</param>
		/// <returns></returns>
		public AnimationHelper AddPositionAnimation(Vector3 position, float time, float inTangent = 0, float outTangent = 0)
		{
			AnimationCurve posXcurve;
			AnimationCurve posYcurve;
			AnimationCurve posZcurve;

			float startTime;
			float fromX;
			float fromY;
			float fromZ;
			float prevInTangent = 0;
			float prevOutTangent = 0;

			// if X curve already exists
			if (_curves.ContainsKey("localPosition.x"))
			{
				posXcurve = _curves["localPosition.x"];
			}
			else
			{
				posXcurve = new AnimationCurve();
				_curves["localPosition.x"] = posXcurve;
			}

			// if Y curve already exists
			if (_curves.ContainsKey("localPosition.y"))
			{
				posYcurve = _curves["localPosition.y"];
			}
			else
			{
				posYcurve = new AnimationCurve();
				_curves["localPosition.y"] = posYcurve;
			}

			// if Z curve already exists
			if (_curves.ContainsKey("localPosition.z"))
			{
				posZcurve = _curves["localPosition.z"];
			}
			else
			{
				posZcurve = new AnimationCurve();
				_curves["localPosition.z"] = posZcurve;
			}
			
			// if not the first animation on x curve
			if (!IsFirstFrame(posXcurve))
			{
				// get value and time of last keyframe
				fromX = GetFrom(posXcurve);
				prevInTangent = GetInTangent(posXcurve);
				prevOutTangent = GetOutTangent(posXcurve);
			}
			else
			{
				// set starting time and value
				fromX = _target.transform.localPosition.x;
			}

			// if not the first animation on y curve
			if (!IsFirstFrame(posYcurve))
			{
				// get value and time of last keyframe
				fromY = GetFrom(posYcurve);
			}
			else
			{
				// set starting time and value
				fromY = _target.transform.localPosition.y;
			}

			// if not the first animation on z curve
			if (!IsFirstFrame(posZcurve))
			{
				// get value and time of last keyframe
				startTime = GetTime(posZcurve);
				fromZ = GetFrom(posZcurve);
			}
			else
			{
				// set starting time and value
				startTime = 0;
				fromZ = _target.transform.localPosition.z;
			}

			// create start keyframes with in/out tangents
			var xKey = new Keyframe(startTime, fromX, prevInTangent, prevOutTangent);
			var yKey = new Keyframe(startTime, fromY, prevInTangent, prevOutTangent);
			var zKey = new Keyframe(startTime, fromZ);

			// set start keyframes
			posXcurve.AddKey(xKey);
			posYcurve.AddKey(yKey);
			posZcurve.AddKey(zKey);

			// create end keyframes with in/out tangents
			var xKeyEnd = new Keyframe(startTime + time, position.x, inTangent, outTangent);
			var ykeyEnd = new Keyframe(startTime + time, position.y, inTangent, outTangent);
			var zkeyEnd = new Keyframe(startTime + time, position.z);

			// set end keyframes
			posXcurve.AddKey(xKeyEnd);
			posYcurve.AddKey(ykeyEnd);
			posZcurve.AddKey(zkeyEnd);

			return this;
		}

		/// <summary>
		/// Adds the scale animation.
		/// </summary>
		/// <param name="scale">The scale.</param>
		/// <param name="time">The time.</param>
		/// <param name="inTangent">The in tangent.</param>
		/// <param name="outTangent">The out tangent.</param>
		/// <returns></returns>
		public AnimationHelper AddScaleAnimation(Vector3 scale, float time, float inTangent = 0, float outTangent = 0)
		{
			AnimationCurve scaleXcurve;
			AnimationCurve scaleYcurve;
			AnimationCurve scaleZcurve;

			float startTime;
			float fromX;
			float fromY;
			float fromZ;
			float prevInTangent = 0;
			float prevOutTangent = 0;

			// if X curve already exists
			if (_curves.ContainsKey("localScale.x"))
			{
				scaleXcurve = _curves["localScale.x"];
			}
			else
			{
				scaleXcurve = new AnimationCurve();
				_curves["localScale.x"] = scaleXcurve;
			}

			// if Y curve already exists
			if (_curves.ContainsKey("localScale.y"))
			{
				scaleYcurve = _curves["localScale.y"];
			}
			else
			{
				scaleYcurve = new AnimationCurve();
				_curves["localScale.y"] = scaleYcurve;
			}

			// if Z curve already exists
			if (_curves.ContainsKey("localScale.z"))
			{
				scaleZcurve = _curves["localScale.z"];
			}
			else
			{
				scaleZcurve = new AnimationCurve();
				_curves["localScale.z"] = scaleZcurve;
			}


			// if not the first animation on x curve
			if (!IsFirstFrame(scaleXcurve))
			{
				// get value and time of last keyframe
				GetTime(scaleXcurve);
				fromX = GetFrom(scaleXcurve);
				prevInTangent = GetInTangent(scaleXcurve);
				prevOutTangent = GetOutTangent(scaleXcurve);
			}
			else
			{
				// set starting time and value
				fromX = _target.transform.localScale.x;
			}

			// if not the first animation on y curve
			if (!IsFirstFrame(scaleYcurve))
			{
				// get value and time of last keyframe
				fromY = GetFrom(scaleYcurve);
			}
			else
			{
				// set starting time and value
				fromY = _target.transform.localScale.y;
			}

			// if not the first animation on z curve
			if (!IsFirstFrame(scaleZcurve))
			{
				// get value and time of last keyframe
				startTime = GetTime(scaleZcurve);
				fromZ = GetFrom(scaleZcurve);
			}
			else
			{
				// set starting time and value
				startTime = 0;
				fromZ = _target.transform.localScale.z;
			}

			// create start keyframes with in/out tangents
			var xKey = new Keyframe(startTime, fromX, prevInTangent, prevOutTangent);
			var yKey = new Keyframe(startTime, fromY, prevInTangent, prevOutTangent);
			var zKey = new Keyframe(startTime, fromZ, prevInTangent, prevOutTangent);

			// set start keyframe
			scaleXcurve.AddKey(xKey);
			scaleYcurve.AddKey(yKey);
			scaleZcurve.AddKey(zKey);

			// create end keyframes with in/out tangents
			var xKeyEnd = new Keyframe(startTime + time, scale.x, inTangent, outTangent);
			var yKeyEnd = new Keyframe(startTime + time, scale.y, inTangent, outTangent);
			var zKeyEnd = new Keyframe(startTime + time, scale.z, inTangent, outTangent);

			// set end keyframe
			scaleXcurve.AddKey(xKeyEnd);
			scaleYcurve.AddKey(yKeyEnd);
			scaleZcurve.AddKey(zKeyEnd);

			return this;
		}

		/// <summary>
		/// Adds the rotation animation.
		/// </summary>
		/// <param name="rotation">The rotation.</param>
		/// <param name="time">The time.</param>
		/// <param name="inTangent">The in tangent.</param>
		/// <param name="outTangent">The out tangent.</param>
		/// <returns></returns>
		public AnimationHelper AddRotationAnimation(Vector3 rotation, float time, float inTangent = 0, float outTangent = 0)
		{
			// convert the given vector3 (desired angle) to quaternion
			var angle = Quaternion.Euler(rotation);

			AnimationCurve rotationXcurve;
			AnimationCurve rotationYcurve;
			AnimationCurve rotationZcurve;
			AnimationCurve rotationWcurve;

			float startTime;
			float fromX;
			float fromY;
			float fromZ;
			float fromW;
			float prevInTangent = 0;
			float prevOutTangent = 0;

			// if X curve already exists
			if (_curves.ContainsKey("localRotation.x"))
			{
				rotationXcurve = _curves["localRotation.x"];
			}
			else
			{
				rotationXcurve = new AnimationCurve();
				_curves["localRotation.x"] = rotationXcurve;
			}

			// if Y curve already exists
			if (_curves.ContainsKey("localRotation.y"))
			{
				rotationYcurve = _curves["localRotation.y"];
			}
			else
			{
				rotationYcurve = new AnimationCurve();
				_curves["localRotation.y"] = rotationYcurve;
			}

			// if Z curve already exists
			if (_curves.ContainsKey("localRotation.z"))
			{
				rotationZcurve = _curves["localRotation.z"];
			}
			else
			{
				rotationZcurve = new AnimationCurve();
				_curves["localRotation.z"] = rotationZcurve;
			}

			// if W curve already exists
			if (_curves.ContainsKey("localRotation.w"))
			{
				rotationWcurve = _curves["localRotation.w"];
			}
			else
			{
				rotationWcurve = new AnimationCurve();
				_curves["localRotation.w"] = rotationWcurve;
			}


			// if not the first animation on x curve
			if (!IsFirstFrame(rotationXcurve))
			{
				// get value and time of last keyframe
				fromX = GetFrom(rotationXcurve);
				prevInTangent = GetInTangent(rotationXcurve);
				prevOutTangent = GetOutTangent(rotationXcurve);
			}
			else
			{
				// set starting time and value
				fromX = _target.transform.localRotation.x;
			}

			// if not the first animation on y curve
			if (!IsFirstFrame(rotationYcurve))
			{
				// get value and time of last keyframe
				fromY = GetFrom(rotationYcurve);
			}
			else
			{
				// set starting time and value
				fromY = _target.transform.localRotation.y;
			}

			// if not the first animation on z curve
			if (!IsFirstFrame(rotationZcurve))
			{
				// get value and time of last keyframe
				fromZ = GetFrom(rotationZcurve);
			}
			else
			{
				// set starting time and value
				fromZ = _target.transform.localRotation.z;
			}

			// if not the first animation on w curve
			if (!IsFirstFrame(rotationWcurve))
			{
				// get value and time of last keyframe
				startTime = GetTime(rotationWcurve);
				fromW = GetFrom(rotationWcurve);
			}
			else
			{
				// set starting time and value
				startTime = 0;
				fromW = _target.transform.localRotation.w;
			}

			// create start keyframes with in/out tangents
			var xKey = new Keyframe(startTime, fromX, prevInTangent, prevOutTangent);
			var yKey = new Keyframe(startTime, fromY, prevInTangent, prevOutTangent);
			var zKey = new Keyframe(startTime, fromZ, prevInTangent, prevOutTangent);
			var wKey = new Keyframe(startTime, fromW, prevInTangent, prevOutTangent);

			// set start keyframe
			rotationXcurve.AddKey(xKey);
			rotationYcurve.AddKey(yKey);
			rotationZcurve.AddKey(zKey);
			rotationWcurve.AddKey(wKey);

			// create end keyframes with in/out tangents
			var xKeyEnd = new Keyframe(startTime + time, angle.x, inTangent, outTangent);
			var yKeyEnd = new Keyframe(startTime + time, angle.y, inTangent, outTangent);
			var zKeyEnd = new Keyframe(startTime + time, angle.z, inTangent, outTangent);
			var wKeyEnd = new Keyframe(startTime + time, angle.w, inTangent, outTangent);

			// set end keyframe
			rotationXcurve.AddKey(xKeyEnd);
			rotationYcurve.AddKey(yKeyEnd);
			rotationZcurve.AddKey(zKeyEnd);
			rotationZcurve.AddKey(wKeyEnd);

			return this;
		}

		/// <summary>
		/// Adds the color animation.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="time">The time.</param>
		/// <param name="inTangent">The in tangent.</param>
		/// <param name="outTangent">The out tangent.</param>
		/// <returns></returns>
		public AnimationHelper AddColorAnimation(Color color, float time, float inTangent = 0, float outTangent = 0)
		{
			AnimationCurve colorRcurve;
			AnimationCurve colorGcurve;
			AnimationCurve colorBcurve;
			AnimationCurve colorAcurve;

			float startTime;
			float fromR;
			float fromG;
			float fromB;
			float fromA;
			float prevInTangent = 0;
			float prevOutTangent = 0;

			// if R curve already exists
			if (_curves.ContainsKey("_Color.r"))
			{
				colorRcurve = _curves["_Color.r"];
			}
			else
			{
				colorRcurve = new AnimationCurve();
				_curves["_Color.r"] = colorRcurve;
			}

			// if G curve already exists
			if (_curves.ContainsKey("_Color.g"))
			{
				colorGcurve = _curves["_Color.g"];
			}
			else
			{
				colorGcurve = new AnimationCurve();
				_curves["_Color.g"] = colorGcurve;
			}

			// if B curve already exists
			if (_curves.ContainsKey("_Color.b"))
			{
				colorBcurve = _curves["_Color.b"];
			}
			else
			{
				colorBcurve = new AnimationCurve();
				_curves["_Color.b"] = colorBcurve;
			}

			// if A curve already exists
			if (_curves.ContainsKey("_Color.a"))
			{
				colorAcurve = _curves["_Color.a"];
			}
			else
			{
				colorAcurve = new AnimationCurve();
				_curves["_Color.a"] = colorAcurve;
			}
			
			// if not the first animation on r curve
			if (!IsFirstFrame(colorRcurve))
			{
				// get value and time of last keyframe
				fromR = GetFrom(colorRcurve);
				prevInTangent = GetInTangent(colorRcurve);
				prevOutTangent = GetOutTangent(colorRcurve);
			}
			else
			{
				// set starting time and value
				fromR = _target.GetComponent<Renderer>().material.color.r;
			}

			// if not the first animation on g curve
			if (!IsFirstFrame(colorGcurve))
			{
				// get value and time of last keyframe
				fromG = GetFrom(colorGcurve);
			}
			else
			{
				// set starting time and value
				fromG = _target.GetComponent<Renderer>().material.color.g;
			}

			// if not the first animation on b curve
			if (!IsFirstFrame(colorBcurve))
			{
				// get value and time of last keyframe
				fromB = GetFrom(colorBcurve);
			}
			else
			{
				// set starting time and value
				fromB = _target.GetComponent<Renderer>().material.color.b;
			}

			// if not the first animation on a curve
			if (!IsFirstFrame(colorAcurve))
			{
				// get value and time of last keyframe
				startTime = GetTime(colorAcurve);
				fromA = GetFrom(colorAcurve);
			}
			else
			{
				// set starting time and value
				startTime = 0;				
				fromA = _target.GetComponent<Renderer>().material.color.a;
			}

			// create start keyframes with in/out tangents
			var rKey = new Keyframe(startTime, fromR, prevInTangent, prevOutTangent);
			var gKey = new Keyframe(startTime, fromG, prevInTangent, prevOutTangent);
			var bKey = new Keyframe(startTime, fromB, prevInTangent, prevOutTangent);
			var aKey = new Keyframe(startTime, fromA, prevInTangent, prevOutTangent);

			// set start keyframe
			colorRcurve.AddKey(rKey);
			colorGcurve.AddKey(gKey);
			colorBcurve.AddKey(bKey);
			colorAcurve.AddKey(aKey);

			// create end keyframes with in/out tangents
			var rKeyEnd = new Keyframe(startTime + time, color.r, inTangent, outTangent);
			var gKeyEnd = new Keyframe(startTime + time, color.g, inTangent, outTangent);
			var bKeyEnd = new Keyframe(startTime + time, color.b, inTangent, outTangent);
			var aKeyEnd = new Keyframe(startTime + time, color.a, inTangent, outTangent);

			// set end keyframe
			colorRcurve.AddKey(rKeyEnd);
			colorGcurve.AddKey(gKeyEnd);
			colorBcurve.AddKey(bKeyEnd);
			colorAcurve.AddKey(aKeyEnd);

			return this;
		}

		/// <summary>
		/// Adds the color animation.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="time">The time.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="inTangent">The in tangent.</param>
		/// <param name="outTangent">The out tangent.</param>
		/// <returns></returns>
		public AnimationHelper AddColorAnimation(Color color, float time, string propertyName, float inTangent = 0,
			float outTangent = 0)
		{
			AnimationCurve colorRcurve;
			AnimationCurve colorGcurve;
			AnimationCurve colorBcurve;
			AnimationCurve colorAcurve;

			float startTime;
			float fromR;
			float fromG;
			float fromB;
			float fromA;
			float prevInTangent = 0;
			float prevOutTangent = 0;

			// if R curve already exists
			if (_curves.ContainsKey(propertyName + ".r"))
			{
				colorRcurve = _curves[propertyName + ".r"];
			}
			else
			{
				colorRcurve = new AnimationCurve();
				_curves[propertyName + ".r"] = colorRcurve;
			}

			// if G curve already exists
			if (_curves.ContainsKey(propertyName + ".g"))
			{
				colorGcurve = _curves[propertyName + ".g"];
			}
			else
			{
				colorGcurve = new AnimationCurve();
				_curves[propertyName + ".g"] = colorGcurve;
			}

			// if B curve already exists
			if (_curves.ContainsKey(propertyName + ".b"))
			{
				colorBcurve = _curves[propertyName + ".b"];
			}
			else
			{
				colorBcurve = new AnimationCurve();
				_curves[propertyName + ".b"] = colorBcurve;
			}

			// if A curve already exists
			if (_curves.ContainsKey(propertyName + ".a"))
			{
				colorAcurve = _curves[propertyName + ".a"];
			}
			else
			{
				colorAcurve = new AnimationCurve();
				_curves[propertyName + ".a"] = colorAcurve;
			}


			// if not the first animation on r curve
			if (!IsFirstFrame(colorRcurve))
			{
				// get value and time of last keyframe
				fromR = GetFrom(colorRcurve);
				prevInTangent = GetInTangent(colorRcurve);
				prevOutTangent = GetOutTangent(colorRcurve);
			}
			else
			{
				// set starting time and value
				fromR = _target.GetComponent<Renderer>().material.color.r;
			}

			// if not the first animation on g curve
			if (!IsFirstFrame(colorGcurve))
			{
				// get value and time of last keyframe
				fromG = GetFrom(colorGcurve);
			}
			else
			{
				// set starting time and value
				fromG = _target.GetComponent<Renderer>().material.color.g;
			}

			// if not the first animation on b curve
			if (!IsFirstFrame(colorBcurve))
			{
				// get value and time of last keyframe
				fromB = GetFrom(colorBcurve);
			}
			else
			{
				// set starting time and value
				fromB = _target.GetComponent<Renderer>().material.color.b;
			}

			// if not the first animation on a curve
			if (!IsFirstFrame(colorAcurve))
			{
				// get value and time of last keyframe
				startTime = GetTime(colorAcurve);
				fromA = GetFrom(colorAcurve);
			}
			else
			{
				// set starting time and value
				startTime = 0;
				fromA = _target.GetComponent<Renderer>().material.color.a;
			}

			// create start keyframes with in/out tangents
			var rKey = new Keyframe(startTime, fromR, prevInTangent, prevOutTangent);
			var gKey = new Keyframe(startTime, fromG, prevInTangent, prevOutTangent);
			var bKey = new Keyframe(startTime, fromB, prevInTangent, prevOutTangent);
			var aKey = new Keyframe(startTime, fromA, prevInTangent, prevOutTangent);

			// set start keyframe
			colorRcurve.AddKey(rKey);
			colorGcurve.AddKey(gKey);
			colorBcurve.AddKey(bKey);
			colorAcurve.AddKey(aKey);

			// create end keyframes with in/out tangents
			var rKeyEnd = new Keyframe(startTime + time, color.r, inTangent, outTangent);
			var gKeyEnd = new Keyframe(startTime + time, color.g, inTangent, outTangent);
			var bKeyEnd = new Keyframe(startTime + time, color.b, inTangent, outTangent);
			var aKeyEnd = new Keyframe(startTime + time, color.a, inTangent, outTangent);

			// set end keyframe
			colorRcurve.AddKey(rKeyEnd);
			colorGcurve.AddKey(gKeyEnd);
			colorBcurve.AddKey(bKeyEnd);
			colorAcurve.AddKey(aKeyEnd);

			return this;
		}

		public AnimationHelper AddOrthographicSizeAnimation(float size, float time, float inTangent = 0, float outTangent = 0)
		{
			AnimationCurve orthographicCurve;

			float startTime;
			float from;
			float prevInTangent = 0;
			float prevOutTangent = 0;

			// if orthographic curve already exists
			if (_curves.ContainsKey("orthographic size"))
			{
				orthographicCurve = _curves["orthographic size"];
			}
			else
			{
				orthographicCurve = new AnimationCurve();
				_curves["orthographic size"] = orthographicCurve;
			}

			// if not the first animation on orthographic curve
			if (!IsFirstFrame(orthographicCurve))
			{
				// get value and time of last keyframe
				startTime = GetTime(orthographicCurve);
				from = GetFrom(orthographicCurve);
				prevInTangent = GetInTangent(orthographicCurve);
				prevOutTangent = GetOutTangent(orthographicCurve);
			}
			else
			{
				// set starting time and value
				startTime = 0;
				from = _target.GetComponent<Camera>().orthographicSize;
			}

			// create start keyframes with in/out tangents
			var key = new Keyframe(startTime, from, prevInTangent, prevOutTangent);

			// set start keyframe
			orthographicCurve.AddKey(key);

			// create end keyframes with in/out tangents
			var keyEnd = new Keyframe(startTime + time, size, inTangent, outTangent);

			// set end keyframe
			orthographicCurve.AddKey(keyEnd);

			return this;
		}

		# region adding animation events

		/// <summary>
		/// Adds the animation event.
		/// </summary>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="messageOptions">The message options.</param>
		/// <param name="time">The time.</param>
		/// <returns></returns>
		public AnimationHelper AddAnimationEvent(string functionName, SendMessageOptions messageOptions, float time)
		{
			var aEvent = new AnimationEvent
			{
				functionName = functionName,
				messageOptions = messageOptions,
				time = time
			};

			_clip.AddEvent(aEvent);
			return this;
		}

		/// <summary>
		/// Adds the animation event.
		/// </summary>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="messageOptions">The message options.</param>
		/// <param name="time">The time.</param>
		/// <param name="intParameter">The int parameter.</param>
		/// <returns></returns>
		public AnimationHelper AddAnimationEvent(string functionName, SendMessageOptions messageOptions, float time,
			int intParameter)
		{
			var aEvent = new AnimationEvent
			{
				functionName = functionName,
				messageOptions = messageOptions,
				time = time,
				intParameter = intParameter
			};

			_clip.AddEvent(aEvent);
			return this;
		}

		/// <summary>
		/// Adds the animation event.
		/// </summary>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="messageOptions">The message options.</param>
		/// <param name="time">The time.</param>
		/// <param name="floatParameter">The float parameter.</param>
		/// <returns></returns>
		public AnimationHelper AddAnimationEvent(string functionName, SendMessageOptions messageOptions, float time,
			float floatParameter)
		{
			var aEvent = new AnimationEvent
			{
				functionName = functionName,
				messageOptions = messageOptions,
				time = time,
				floatParameter = floatParameter
			};

			_clip.AddEvent(aEvent);
			return this;
		}

		/// <summary>
		/// Adds the animation event.
		/// </summary>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="messageOptions">The message options.</param>
		/// <param name="time">The time.</param>
		/// <param name="stringParameter">The string parameter.</param>
		/// <returns></returns>
		public AnimationHelper AddAnimationEvent(string functionName, SendMessageOptions messageOptions, float time,
			string stringParameter)
		{
			var aEvent = new AnimationEvent
			{
				functionName = functionName,
				messageOptions = messageOptions,
				time = time,
				stringParameter = stringParameter
			};

			_clip.AddEvent(aEvent);
			return this;
		}

		/// <summary>
		/// Adds the animation event.
		/// </summary>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="messageOptions">The message options.</param>
		/// <param name="time">The time.</param>
		/// <param name="objectReferenceParameter">The object reference parameter.</param>
		/// <returns></returns>
		public AnimationHelper AddAnimationEvent(string functionName, SendMessageOptions messageOptions, float time,
			Object objectReferenceParameter)
		{
			var aEvent = new AnimationEvent
			{
				functionName = functionName,
				messageOptions = messageOptions,
				time = time,
				objectReferenceParameter = objectReferenceParameter
			};

			_clip.AddEvent(aEvent);
			return this;
		}

		#endregion
	}
}