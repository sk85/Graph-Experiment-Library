#pragma once

/// <summary>
/// �m�[�h��\����{�N���X
/// �e�m�[�h���m�[�h�̑����ȉ��̐����Ɉ�ӂɊ��蓖�ĂĂ���Ύ����͎��R��
/// </summary>
class SNode
{
public:

	/// <summary>
	/// �m�[�h�̒ʂ��ԍ����擾
	/// 0 �` NodeNum�Ƀm�[�h����ӂɊ��蓖��
	/// </summary>
	/// <returns>�ʂ��ԍ�</returns>
	virtual uint32_t GetIndex() = 0;

	/// <summary>
	/// �m�[�h�A�h���X���Z�b�g
	/// </summary>
	virtual void SetAddr(uint32_t addr) = 0;
};
