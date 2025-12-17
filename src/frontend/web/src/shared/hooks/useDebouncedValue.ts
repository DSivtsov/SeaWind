import { useEffect, useState } from "react";

/** Custom hook (typed) + cleanup in useEffect */
export function useDebouncedValue<T>(value: T, delayMs: number) {
    const [debounced, setDebounced] = useState<T>(value);

    useEffect(() => {
        const id = window.setTimeout(() => setDebounced(value), delayMs);
        return () => window.clearTimeout(id);
    }, [value, delayMs]);

    return debounced;
}
