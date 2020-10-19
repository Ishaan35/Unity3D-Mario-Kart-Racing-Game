const char * UnityAdsCopyString(const char * string) {
    char * copy = (char *)malloc(strlen(string) + 1);
    strcpy(copy, string);
    return copy;
}

/**
 * Returns the size of an Il2CppString
 */
size_t Il2CppStringLen(const ushort* str) {
    const ushort* start = str;
    while (*str) ++str;
    return str - start;
}

/**
 * Converts an ushort string to an NSString
 */
NSString* NSStringFromIl2CppString(const ushort* str) {
    size_t len = Il2CppStringLen(str);
    return [[NSString alloc] initWithBytes:(const void*)str
                                    length:sizeof(ushort) * len
                                  encoding:NSUTF16LittleEndianStringEncoding];
}
