using UnityEngine;
using System.Collections;

public class FHResultCode
{
	public const int OK = 0;
	public const int FAILED = 1;
	public const int NOT_FOUND = 2;
	public const int PENDING = 3;
	public const int CLOSED = 4;
    public const int TIME_OUT = 5;
	public const int EXPIRED = 6;

	public const int DATABASE_ERROR = 11;

	public const int CANNOT_DO_ACTION = 101;
	public const int REACH_DAILY_LIMIT = 102;

	public const int NOT_CONNECT = 901;
	public const int HTTP_ERROR = 902;
}