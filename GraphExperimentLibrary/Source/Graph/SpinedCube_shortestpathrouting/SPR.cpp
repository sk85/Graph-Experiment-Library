#include <iostream>

#include "SPR.h"


using namespace std;

Expansion SPR::GetMinimalExpansion(Node s, Node d, int n) {
	ulong type1 = s.GetType();
	ulong type2 = d.GetType();
	Expansion MinExp, tmpExp;

	MinExp = AllType(s, d, n);

	if (type1 == type2) {				// �����^�C�v
		if (type1 == 0b00) {
			if (MinExp > (tmpExp = SingleType(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = DoubleType_0001(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = DoubleType_0010(s, d, n))) MinExp = tmpExp;
		}
		else if (type1 == 0b01) {
			if (MinExp > (tmpExp = SingleType(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = DoubleType_0001(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = DoubleType_0111(s, d, n))) MinExp = tmpExp;
		}
		else if (type1 == 0b10) {
			if (MinExp > (tmpExp = SingleType(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = DoubleType_0010(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = DoubleType_1011(s, d, n))) MinExp = tmpExp;
		}
		else {
			if (MinExp > (tmpExp = SingleType(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = DoubleType_1011(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = DoubleType_0111(s, d, n))) MinExp = tmpExp;
		}
	}
	else if ((type1 ^ type2) == 0b11) {	// �Ίp
		if (type1 == 0b00 || type1 == 0b11) {
			if (MinExp > (tmpExp = TripleType_000111(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = TripleType_001011(s, d, n))) MinExp = tmpExp;
		}
		else {
			if (MinExp > (tmpExp = TripleType_000110(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = TripleType_011011(s, d, n))) MinExp = tmpExp;
		}
	}
	else if ((type1 ^ type2) == 0b10) {	// ��10
		if (type1 == 0b00 || type1 == 0b10) {
			if (MinExp > (tmpExp = DoubleType_0010(s, d, n))) MinExp = tmpExp;
		}
		else {
			if (MinExp > (tmpExp = DoubleType_0111(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = TripleType_000111(s, d, n))) MinExp = tmpExp;
			if (MinExp > (tmpExp = TripleType_011011(s, d, n))) MinExp = tmpExp;
		}
	}
	else {								// ��01
		if ((type1 == 0b00) || (type1 == 0b01)) {
			if (MinExp > (tmpExp = DoubleType_0001(s, d, n))) MinExp = tmpExp;
		}
		else {
			if (MinExp > (tmpExp = DoubleType_1011(s, d, n))) MinExp = tmpExp;
		}
	}
	return MinExp;
}


/*	�^�C�v���̏ꍇ	*/
Expansion SPR::SingleType(Node s, Node d, int n) {
	Expansion exp;
	Node c = s ^ d;
	ulong type = s.GetType();

	// ��31�`4�r�b�g
	int index = n - 1;
	while (index >= 4) {
		if (c[index]) {
			Node bin = GetBin(type, index);
			exp += bin;
			c.Pop(bin);
		}
		--index;
	}

	// ��3�`0�r�b�g
	ulong tmp = c.Subsequence(3, 2);
	if (tmp == 0b11) {	// 1100�̏ꍇ
		if (type & 0b01) {	// 01,11�^�C�v�̏ꍇ
			exp += 0b1100;
		}
		else {	// 00,10�^�C�v�̏ꍇ
			exp += 0b1000;
			exp += 0b0100;
		}
	}
	else if (tmp != 0b00) {	// 1000��0100�̏ꍇ
		if (type & 0b01) {	// 01,11�^�C�v�̏ꍇ
			return NULL;
		}
		else {	// 00,10�^�C�v�̏ꍇ
			exp += c;
		}
	}

	return exp;
}



/*	0001�̏ꍇ	*/
Expansion SPR::DoubleType_0001(Node s, Node d, int n) {
	Expansion exp;
	Node c = s ^ d;

	// ��31�`4�r�b�g
	int index = n - 1;
	while (index >= 4) {
		if (c[index]) {
			Node bin = c[index - 2] ? GetBin(0b01, index) : GetBin(0b00, index);
			exp += bin;
			c.Pop(bin);
		}
		--index;
	}

	// ��3, 2�r�b�g
	ulong tmp = c.Subsequence(3, 2);
	if (tmp != 0b00) {
		exp += tmp << 2;
	}

	// ��1, 0�r�b�g
	if (s.GetType() == d.GetType()) {
		exp += 0b01;
	}
	exp += 0b01;

	return exp;
}



/*	0010�̏ꍇ	*/
Expansion SPR::DoubleType_0010(Node s, Node d, int n) {
	Expansion exp;
	Node c = s ^ d;

	// ��31�`4�r�b�g
	int index = n - 1;
	while (index >= 4) {
		if (c[index]) {
			Node bin = c[index - 1] ? GetBin(0b10, index) : GetBin(0b00, index);
			exp += bin;
			c.Pop(bin);
		}
		--index;
	}

	// ��3, 2�r�b�g
	if (c[3]) {
		exp += 0b1000;
	}
	if (c[2]) {
		exp += 0b0100;
	}

	// ��1, 0�r�b�g
	if (s.GetType() == d.GetType()) {
		exp += 0b10;
	}
	exp += 0b10;

	return exp;
}



/*	0111�̏ꍇ
DoubleType_0111			�{��
DoubleType_0111_Sub1	a/1a
DoubleType_0111_Sub2	a/a^,1a^	*/
// �{��
Expansion SPR::DoubleType_0111(Node s, Node d, int n) {
	Expansion exp;
	Node c = s ^ d;

	// ��n-1�r�b�g��1�̏ꍇ
	Node bin;
	if (n > 4 && c[n - 1]) {
		Expansion expA, expB;
		if (c[n - 2]) {	// 11c...�̏ꍇ
			expA += GetBin(0b11, n - 1);
			expB += GetBin(0b01, n - 1);
			c ^= GetBin(0b11, n - 1);
		}
		else {						// 10c...�̏ꍇ
			expA += GetBin(0b01, n - 1);
			expB += GetBin(0b11, n - 1);
			c ^= GetBin(0b01, n - 1);
		}
		c = DoubleType_0111_Sub1(c, n - 3, expA, expB, &exp);
	}

	// ��n-2�`5�r�b�g�܂�
	int index = n - 2;
	while (index > 4) {
		if (c[index]) {
			if (c.Subsequence(index, 2) == 0b11) {			// 11c...�̏ꍇ
				Expansion expX = Expansion(GetBin(0b11, index));
				Expansion expY = Expansion(GetBin(0b01, index));
				c.Pop(0b111, index);
				c = DoubleType_0111_Sub1(c, index - 2, expX, expY, &exp);
			}
			else if (c.Subsequence(index, 3) == 0b101) {	// 101d...�̏ꍇ
				Expansion expX = Expansion(GetBin(0b01, index));
				Expansion expY = Expansion(GetBin(0b11, index));
				c.Pop(0b101, index);
				c = DoubleType_0111_Sub1(c, index - 2, expX, expY, &exp);
			}
			else if (c.Subsequence(index, 4) == 0b1001) {	// 1001e...�̏ꍇ
				Expansion expX = Expansion(GetBin(0b01, index));
				Expansion expY = Expansion(GetBin(0b11, index));
				c.Pop(0b101, index);
				c = DoubleType_0111_Sub1(c, index - 2, expX, expY, &exp);
			}
			else {	// 1000e...�̏ꍇ
				Expansion expX = Expansion(GetBin(0b01, index + 1)) + GetBin(0b11, index + 1);
				Expansion expY = Expansion(GetBin(0b01, index)) + GetBin(0b01, index - 2);
				Expansion expZ = Expansion(GetBin(0b01, index)) + GetBin(0b11, index - 2);
				c.Pop(0b1, index);
				c = DoubleType_0111_Sub2(c, index - 4, expX, expY, expZ, &exp);
			}
		}
		--index;
	}

	// ��4�`2�r�b�g
	Node tmp = c.Subsequence(4, 2);
	if (tmp != 0) {
		if (tmp == 0b11) {
			tmp = 0b11100;
		}
		else if (tmp == 0b10) {
			tmp = 0b10100;
		}
		else {
			tmp = 0b1100;
		}
		exp += tmp;
		c.Pop(tmp);
	}


	// ��(2)1�`0�r�b�g
	exp += c[2] ? 0b0110 : 0b0010;	// ��2�r�b�g��1�Ȃ�0110�A0�Ȃ�0010
	if (s.GetType() == d.GetType()) {	// s,d�������^�C�v�Ȃ�0010��ǉ�
		exp += 0b0010;
	}

	return exp;
}

// �T�u���[�`��1
Node SPR::DoubleType_0111_Sub1(Node c, int index, Expansion expA, Expansion expB, Expansion *exp) {
	// ������6�����Ȃ��a <= 1a��������
	if (index < 5) {
		*exp += expA;
		return c;
	}

	// ������6�ȏ�̂Ƃ�
	if (c[index]) {
		if (c[index - 1]) {				// abcdef = 11cdef...�̏ꍇ
			Expansion expX = expB + GetBin(0b11, index + 1);
			Expansion expY = expA + GetBin(0b11, index);
			Expansion expZ = expA + GetBin(0b01, index);
			c.Pop(0b11, index);
			c = DoubleType_0111_Sub2(c, index - 2, expX, expY, expZ, exp);
		}
		else {							// abcdef = 10cdef...�̏ꍇ
			*exp += expA;
		}
	}
	else {
		if (c[index - 1]) {
			if (c[index - 2]) {
				if (c[index - 3]) {		// abcdef = 0111ef...�̏ꍇ
					*exp += expA;
				}
				else {
					if (c[index - 4]) {	// abcdef = 01101f...�̏ꍇ
						*exp += expA;
					}
					else {				// abcdef = 01100f...�̏ꍇ
						if (index < 7) {	/// ������7�ȉ��Ȃ獶�L���m��
							*exp += expA;
						}
						else {
							Expansion expX = expB + GetBin(0b11, index + 1) + GetBin(0b01, index);
							Expansion expY = expA + GetBin(0b11, index) + GetBin(0b01, index - 3);
							Expansion expZ = expA + GetBin(0b11, index) + GetBin(0b01, index - 3);///////////
							c.Pop(0b11, index - 1);
							c = DoubleType_0111_Sub2(c, index - 5, expX, expY, expZ, exp);
						}
					}
				}

			}
			else {						// abcdef = 010def...�̏ꍇ
				Expansion expX = expB + GetBin(0b01, index + 1);
				Expansion expY = expA + GetBin(0b01, index - 1);
				Expansion expZ = expA + GetBin(0b11, index - 1);
				c.Pop(0b1, index - 1);
				c = DoubleType_0111_Sub2(c, index - 3, expX, expY, expZ, exp);
			}
		}
		else {
			if (c[index - 2]) {
				if (c[index - 3]) {
					if (c[index - 4]) {	// abcdef = 00111f...�̏ꍇ
						*exp += expA;
					}
					else {
						if (c[index - 5]) {	// abcdefg = 001101g...�̏ꍇ
							*exp += expA;
						}
						else {				// abcdefg = 001100g...�̏ꍇ
							if (index < 8) {	/// ������8�ȉ��Ȃ獶�L���m��
								*exp += expA;
							}
							else {
								Expansion expX = expB + GetBin(0b01, index + 1) + GetBin(0b11, index - 1);
								Expansion expY = expA + GetBin(0b11, index - 2) + GetBin(0b01, index - 3);
								Expansion expZ = expA + GetBin(0b11, index - 2) + GetBin(0b11, index - 3);
								c.Pop(0b11, index - 2);
								c = DoubleType_0111_Sub2(c, index - 6, expX, expY, expZ, exp);
							}
						}
					}
				}
				else {					// abcdef = 0010ef...�̏ꍇ
					*exp += expA;
				}
			}
			else {						// abcdef = 000def...�̏ꍇ
				*exp += expA;
			}
		}
	}
	return c;
}

// �T�u���[�`��2
Node SPR::DoubleType_0111_Sub2(Node c, int index, Expansion expA, Expansion expB, Expansion expC, Expansion *exp) {
	// ������5�����Ȃ�Έ�ʓI�ȏ����͕s��
	// ����āA���������X�ɏ���
	if (index == 3) {
		if (c[3]) {
			c.Pop(1, 3);
			*exp += expB;
		}
		else {
			*exp += expA;
		}
		return c;
	}
	else if (index < 3) {
		*exp += expA;
		return c;
	}

	// ������6�ȏ�̂Ƃ�
	if (c[index]) {
		if (c[index - 1]) {
			if (c[index - 2]) {	// abcd=...111d...�̏ꍇ
				Expansion expX = expA + GetBin(0b11, index);
				Expansion expY = expB + GetBin(0b11, index);
				Expansion expZ = expB + GetBin(0b01, index);
				c.Pop(0b111, index);
				c = DoubleType_0111_Sub2(c, index - 3, expX, expY, expZ, exp);
			}
			else {				// abcd=...110d...�̏ꍇ
				Expansion expX = expC + GetBin(0b01, index + 1);
				Expansion expY = expB + GetBin(0b01, index);
				Expansion expZ = expB + GetBin(0b11, index);
				c.Pop(0b110, index);
				c = DoubleType_0111_Sub2(c, index - 3, expX, expY, expZ, exp);
			}
		}
		else {					// abcd=...10cd...�̏ꍇ
			c.Pop(0b1, index);
			c = DoubleType_0111_Sub1(c, index - 1, expB, expA, exp);
		}
	}
	else {
		if (c[index - 1]) {
			if (c[index - 2]) {	// abcd=...011d...�̏ꍇ
				Expansion expX = expB + GetBin(0b11, index);
				Expansion expY = expA + GetBin(0b11, index);
				Expansion expZ = expA + GetBin(0b01, index);
				c.Pop(0b11, index - 1);
				c = DoubleType_0111_Sub2(c, index - 3, expX, expY, expZ, exp);
			}
			else {				// abcd=...010d...�̏ꍇ
				Expansion expX = expC + GetBin(0b11, index + 1);
				Expansion expY = expA + GetBin(0b01, index - 1);
				Expansion expZ = expA + GetBin(0b11, index - 1);
				c.Pop(0b10, index - 1);
				c = DoubleType_0111_Sub2(c, index - 3, expX, expY, expZ, exp);
			}
		}
		else {					// abcd=...00cd...�̏ꍇ
			c = DoubleType_0111_Sub1(c, index - 1, expA, expB, exp);
		}
	}

	return c;
}



/*	1011�̏ꍇ
DoubleType_1011			�{��
DoubleType_1011_Sub1	a/1a
DoubleType_1011_Sub2	a/11a	*/
// �{��
Expansion SPR::DoubleType_1011(Node s, Node d, int n) {
	Expansion exp;
	Node c = s ^ d;

	// ��31�`4�r�b�g
	int index = n - 1;
	Node bin;
	Expansion expX;
	Expansion expY;
	while (index >= 4) {
		if (c[index]) {
			switch (c.Subsequence(index, 3)) {
			case 0b100:	// 100...�̏ꍇ
				bin = GetBin(0b11, index);
				exp += bin;
				c.Pop(bin);
				break;
			case 0b101:	// 101...�̏ꍇ
				bin = GetBin(0b10, index);
				exp += bin;
				c.Pop(bin);
				break;
			case 0b110:	// 110...�̏ꍇ
				bin = GetBin(0b10, index);
				expX = Expansion(bin);
				expY = Expansion(GetBin(0b11, index));
				c.Pop(bin);
				c = DoubleType_1011_Sub1(c, index - 3, expX, expY, &exp);
				break;
			default:	// 100...�̏ꍇ
				bin = GetBin(0b11, index);
				expX = Expansion(bin);
				expY = Expansion(GetBin(0b10, index));
				c.Pop(bin);
				c = DoubleType_1011_Sub1(c, index - 3, expX, expY, &exp);
				break;
			}
		}
		--index;
	}
	// ��3�`2�r�b�g
	ulong tmp = c.GetAddr() & 0b1100;
	if (tmp) {
		exp += tmp;
	}

	// ��1�`0�r�b�g
	exp += 0b0001;
	if (s.GetType() == d.GetType()) {
		exp += 0b0001;
	}

	return exp;
}

// Subroutin 1
Node SPR::DoubleType_1011_Sub1(Node c, int index, Expansion expA, Expansion expB, Expansion *exp) {
	// if c is shorter than 5 (index is smaller than 4) then a <= 1a
	if (index <= 3) {
		*exp += expA;
		return c;
	}
	Node bin;
	Expansion expX, expY;
	// if c is longer than 4(index is bigger than 3)
	if ((c[index] == 0) || (c[index] == 1 && c[index - 1] == 1)) {	// abcd... = 0bcd... or 11cd...
		*exp += expA;
	}
	else {
		switch (c.Subsequence(index, 3)) {
		case 0b100:	// abcd... = 100d...
			expX = expB + GetBin(0b10, index + 1);
			expY = expA + GetBin(0b11, index);
			c.Pop(0b1, index);
			c = DoubleType_1011_Sub2(c, index - 3, expX, expY, exp);
			break;
		case 0b101:	// abcd... = 101d...
			*exp += expB;
			c.Pop(0b1, index + 1);
			break;
		}
	}
	return c;
}

// Subroutin2
Node SPR::DoubleType_1011_Sub2(Node c, int index, Expansion expA, Expansion expB, Expansion *exp) {
	// if c is shorter than 5 (index is smaller than 4) then a <= 1a
	if (index <= 3) {
		*exp += expA;
		return c;
	}

	// if c is longer than 4 (index is bigger than 3)
	Node bin;
	Expansion expX, expY;
	if ((c[index] == 0) || (c[index] == 1 && c[index - 1] == 1)) {	// abcd... = 0bcd... or 11cd...
		*exp += expA;
	}
	else {
		switch (c.Subsequence(index, 3)) {
		case 0b100:	// abcd... = 100d...
			expX = expB + GetBin(0b11, index + 2);
			expY = expA + GetBin(0b11, index);
			c.Pop(1, index);
			c = DoubleType_1011_Sub2(c, index - 3, expX, expY, exp);
			break;
		case 0b101:	// abcd... = 101d...
			expX = expB + GetBin(0b11, index + 2);
			expY = expA + GetBin(0b10, index);
			c.Pop(1, index);
			c = DoubleType_1011_Sub1(c, index - 2, expX, expY, exp);
			break;
		}
	}
	return c;
}



/*	000110�̏ꍇ	*/
// �{��
Expansion SPR::TripleType_000110(Node s, Node d, int n) {
	Expansion exp;
	Node c = s ^ d;

	// ��31�`4�r�b�g
	int index = n - 1;
	while (index >= 4) {
		if (c[index]) {
			Node bin;
			if (c[index - 1]) {		// abcd... = 11cd...
				bin = GetBin(0b10, index);
			}
			else {
				if (c[index - 2]) {	// abcd... = 101d...
					bin = GetBin(0b01, index);
				}
				else {				// abcd... = 100d...
					bin = GetBin(0b00, index);
				}
			}
			c.Pop(bin);
			exp += bin;
		}
		--index;
	}

	// ��3�`2�r�b�g
	int tmp = c.GetAddr() & 0b1100;
	if (tmp) {
		exp += tmp;
	}

	// ��1�`0�r�b�g
	exp += 0b0010;
	exp += 0b0001;

	return exp;
}



/*	000111�̏ꍇ
DoubleType_000111			�{��
DoubleType_000111_Sub1	a/a^
DoubleType_000111_Sub2	a/10a	*/
// main
Expansion SPR::TripleType_000111(Node s, Node d, int n) {
	Expansion exp;
	Node c = s ^ d;

	// ��31�`4�r�b�g
	int index = n - 1;
	while (index >= 4) {
		if (c[index]) {
			if (c[index - 1]) {
				if (c[index - 2]) {	// abcd... = 111d...
					Node bin = GetBin(0b11, index);
					c.Pop(bin);
					exp += bin;
				}
				else {	// abcd... = 110d...
					Expansion expX = GetBin(0b11, index);
					Expansion expY = GetBin(0b00, index);
					c.Pop(0b11, index);
					c = TripleType_000111_Sub2(c, index - 3, expX, expY, &exp);
				}
			}
			else {	// abcd... = 10cd...
				Expansion expX = GetBin(0b00, index);
				Expansion expY = GetBin(0b01, index);
				c.Pop(1, index);
				c = TripleType_000111_Sub1(c, index - 2, expX, expY, &exp);
			}
		}
		--index;
	}

	// ��3�`2�r�b�g
	if (c[3]) {
		exp += 0b1000;
	}

	// ��2�`0�r�b�g
	if (c[2]) {
		exp += 0b0110;
	}
	else {
		exp += 0b0010;
	}
	exp += 0b0001;
	if ((s.GetType() ^ d.GetType()) == 0b10) {
		exp += 0b0001;
	}

	return exp;
}

// Subroutin 1
Node SPR::TripleType_000111_Sub1(Node c, int index, Expansion expA, Expansion expB, Expansion *exp) {
	if (index <= 4) {
		if (c[index]) {
			*exp += expB;
			c.Pop(1, index);
		}
		else {
			*exp += expA;
		}
	}
	else {
		if (c[index - 1]) {
			if (c[index - 2]) {	// abcd... = a11d...
				if (c[index]) {
					Expansion expX = expA + GetBin(0b11, index);
					Expansion expY = expB + GetBin(0b11, index - 1);
					c.Pop(0b111, index);
					c = TripleType_000111_Sub1(c, index - 3, expX, expY, exp);
				}
				else {
					Expansion expX = expB + GetBin(0b11, index);
					Expansion expY = expA + GetBin(0b11, index - 1);
					c.Pop(0b11, index - 1);
					c = TripleType_000111_Sub1(c, index - 3, expX, expY, exp);
				}
			}
			else {	// abcd... = a10d...
				if (c[index]) {
					*exp += expB;
					c.Pop(1, index);
				}
				else {
					*exp += expA;
				}
			}
		}
		else {	// abcd... = a0cd...
			if (c[index]) {
				*exp += expB;
				c.Pop(1, index);
			}
			else {
				*exp += expA;
			}
		}
	}
	return c;
}

// Subroutin 2
Node SPR::TripleType_000111_Sub2(Node c, int index, Expansion expA, Expansion expB, Expansion *exp) {
	if (index <= 1) {
		c.Pop(1, index + 1);
		*exp += expA;
	}
	else if (index == 2) {
		*exp += expA + Node(0b1000);
	}
	else {
		if (c[index]) {	// a = 1
			Expansion expX = expB + GetBin(0b01, index + 2);
			Expansion expY = expA + GetBin(0b11, index + 1);
			c.Pop(1, index);
			c = TripleType_000111_Sub1(c, index - 1, expX, expY, exp);
		}
		else {	// a = 0
			c.Pop(1, index + 1);
			*exp += expA;
		}
	}

	return c;
}



/*	001011�̏ꍇ	*/
// main
Expansion SPR::TripleType_001011(Node s, Node d, int n) {
	Expansion exp;
	Node c = s ^ d;

	// ��31�`4�r�b�g
	int index = n - 1;
	while (index >= 4) {
		if (c[index]) {
			if (c[index - 1] == 0) {	// abcd... = 10cd...
				Node bin = GetBin(0b00, index);
				c.Pop(bin);
				exp += bin;
			}
			else if (c[index - 2]) {		// abcd... = 111d...
				Node bin = GetBin(0b11, index);
				c.Pop(bin);
				exp += bin;
			}
			else {						// abcd... = 110d...
				Node bin = GetBin(0b10, index);
				c.Pop(bin);
				exp += bin;
			}
		}
		--index;
	}

	// ��3~2�r�b�g
	if (c[3]) {
		if (c[2]) {
			exp += 0b1100;
		}
		else {
			exp += 0b1000;
		}
	}
	else {
		if (c[2]) {
			exp += 0b0100;
		}
	}

	// ��1 �` 0�r�b�g
	exp += 0b0010;
	exp += 0b0001;

	return exp;
}



/*	011011�̏ꍇ	*/
// main
Expansion SPR::TripleType_011011(Node s, Node d, int n) {
	Expansion exp;
	Node c = s ^ d;

	// ��31�`4�r�b�g
	int index = n - 1;
	while (index >= 4) {
		if (c[index]) {
			if (c[index - 1] == 0) {	// abcd... = 10cd...
				Node bin = GetBin(0b01, index);
				Expansion expX = bin;
				Expansion expY = GetBin(0b11, index);
				c.Pop(bin);
				c = TripleType_011011_Sub(c, index - 2, expX, expY, &exp);
			}
			else if (c[index - 2]) {		// abcd... = 111d...
				Node bin = GetBin(0b11, index);
				Expansion expX = bin;
				Expansion expY = GetBin(0b10, index);
				c.Pop(bin);
				c = TripleType_011011_Sub(c, index - 3, expX, expY, &exp);
			}
			else {						// abcd... = 110d...
				Node bin = GetBin(0b10, index);
				Expansion expX = bin;
				Expansion expY = GetBin(0b11, index);
				c.Pop(bin);
				c = TripleType_011011_Sub(c, index - 3, expX, expY, &exp);
			}
		}
		--index;
	}

	// ��3�r�b�g
	if (c[3]) {
		exp += 0b1000;
	}

	// ��2�`1�r�b�g
	if (c[2]) {
		exp += 0b0110;
	}
	else {
		exp += 0b0010;
	}

	// ��0�r�b�g
	exp += 0b0001;
	if ((s.GetType() ^ d.GetType()) == 0b10) {
		exp += 0b0001;
	}

	return exp;
}

// Subroutin
Node SPR::TripleType_011011_Sub(Node c, int index, Expansion expA, Expansion expB, Expansion *exp) {
	//cout << c << "\t" << index << endl;
	// if c is shorter than 6 (index is smaller than 5) then a <= 1a
	if (index <= 4) {
		*exp += expA;
		return c;
	}

	// if c is longer than 5 (index is bigger than 4)
	// ��
	ulong tmp = c.Subsequence(index, 2);
	if (tmp == 0b00 || tmp == 0b11) {	// abcde... = 00cde..., 11cde...
		*exp += expA;
	}
	// �O��
	tmp = c.Subsequence(index, 3);
	if (tmp == 0b011) {					// abcde... = 011de...
		*exp += expA;
	}
	else if (tmp == 0b100) {				// abcde... = 100de...
		expA += GetBin(0b01, index);
		expB += GetBin(0b10, index + 1);
		c.Pop(0b01, index);
		c = TripleType_011011_Sub(c, index - 3, expB, expA, exp);
	}
	else if (tmp == 0b101) {			// abcde... = 101de...
		Node bin = GetBin(0b01, index);
		expA += bin;
		expB += GetBin(0b10, index + 1);
		c.Pop(bin);
		c = TripleType_011011_Sub(c, index - 3, expA, expB, exp);
	}
	// �l��
	tmp = c.Subsequence(index, 4);
	if (tmp == 0b0100) {					// abcde... = 0100e...
		expA += GetBin(0b01, index - 1);
		expB += GetBin(0b01, index + 1);
		c.Pop(0b01, index - 1);
		c = TripleType_011011_Sub(c, index - 4, expB, expA, exp);
	}
	else if (tmp == 0b0101) {				// abcde... = 0101e...
		Node bin = GetBin(0b01, index - 1);
		expA += bin;
		expB += GetBin(0b01, index + 1);
		c.Pop(bin);
		c = TripleType_011011_Sub(c, index - 4, expA, expB, exp);
	}

	return c;
}



/*	all�̏ꍇ	*/
// main
Expansion SPR::AllType(Node s, Node d, int n) {
	Expansion exp;
	Node c = s ^ d;

	// ��31�`4�r�b�g
	int index = n - 1;
	while (index >= 4) {
		if (c[index]) {
			ulong type = c.Subsequence(index - 1, 2);
			Node bin = GetBin(type, index);
			c.Pop(bin);
			exp += bin;
		}
		--index;
	}

	ulong type1 = s.GetType();
	ulong type2 = d.GetType();

	// ��3�r�b�g
	ulong tmp = c.Subsequence(3, 2);
	if (tmp == 0b11 || tmp == 0b10) {
		Node tmp2 = tmp << 2;
		exp += tmp2;
		c.Pop(tmp2);
	}


	// ��2�r�b�g
	if (c[2]) {
		if ((type1 == 0b01 && type2 == 0b11) || (type1 == 0b11 && type2 == 0b01)) {
			exp += 0b0100;
			exp += 0b0010;
		}
		else {
			exp += 0b0110;
		}
	}
	else {
		exp += 0b0010;
	}

	// ��2 �` 0�r�b�g
	if ((type1 ^ type2) == 0b10) {
		exp += 0b0001;
	}
	else if ((type1 ^ type2) == 0b01) {
		exp += 0b0010;
	}
	else if (type1 == type2) {
		exp += 0b0010;
		exp += 0b0001;
	}
	else {
		exp += 0b0010;
		exp += 0b0010;
	}
	exp += 0b0001;

	return exp;
}