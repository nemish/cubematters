using System;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace ParadoxNotion.Serialization{

	///Serialized MethodInfo
	[Serializable]
	public class SerializedMethodInfo{

		[SerializeField]
		private string _returnInfo;		
		[SerializeField]
		private string _baseInfo;
		[SerializeField]
		private string _paramsInfo;
		
		private MethodInfo _method;

		//required
		public SerializedMethodInfo(){}
		///Serialize a new MethodInfo
		public SerializedMethodInfo(MethodInfo method){
			_returnInfo = method.ReturnType.FullName;
			_baseInfo = string.Format("{0}|{1}", method.RTReflectedType().FullName, method.Name);
			_paramsInfo = string.Join("|", method.GetParameters().Select(p => p.ParameterType.FullName).ToArray() );
		}

		///Deserialize and return target MethodInfo.
		public MethodInfo Get(){
			if (_method == null && _baseInfo != null){
				_method = GetOriginal();
				if (_method == null){
					_method = GetFinal();
				}
			}

			return _method;
		}

		//original method serialized
		MethodInfo GetOriginal(){

			var type = ReflectionTools.GetType(_baseInfo.Split('|')[0]);
			if (type == null){
				return null;
			}

			var name = _baseInfo.Split('|')[1];
			var paramTypeNames = string.IsNullOrEmpty(_paramsInfo)? null : _paramsInfo.Split('|');
			var parameters = paramTypeNames == null? new Type[]{} : paramTypeNames.Select(n => ReflectionTools.GetType(n)).ToArray();

			var returnType = string.IsNullOrEmpty(_returnInfo)? null : ReflectionTools.GetType(_returnInfo);
			
			_method = type.RTGetMethod(name, parameters);
			if (returnType != null){
				if (_method != null && returnType != _method.ReturnType){
					_method = null;
				}
			}

			return _method;
		}

		//resolve to final method
		MethodInfo GetFinal(){

			var type = ReflectionTools.GetType(_baseInfo.Split('|')[0]);
			if (type == null){
				return null;
			}
			var name = _baseInfo.Split('|')[1];
			try {return _method = type.RTGetMethod(name);} //ambiguous
			catch {return null;}
		}


		//Are the original and finaly resolve methods different?
		//If neither original nor final can be resolved, this returns false. Not changed, but rather not found at all.
		public bool HasChanged(){
			return GetOriginal() == null;
		}

		///Returns the serialized method information.
		public string GetMethodString(){
			return string.Format("{0} ({1})", _baseInfo.Replace("|", "."), _paramsInfo.Replace("|", ", "));
		}
	}
}