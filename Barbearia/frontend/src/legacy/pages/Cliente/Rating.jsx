import { Star } from "lucide-react";

export default function Rating({ value, size = 16 }) {
    return (
        <div className="flex items-center gap-0.5">
            {[0, 1, 2, 3, 4].map((index) => {
                const fill = Math.max(
                    0,
                    Math.min(100, (value - index) * 100)
                );

                return (
                    <div
                        key={index}
                        className="relative"
                        style={{ width: size, height: size }}
                    >
                        {/* Estrela cinza */}
                        <Star
                            size={size}
                            className="absolute text-gray-300"
                        />

                        {/* Parte preenchida */}
                        <div
                            className="absolute overflow-hidden"
                            style={{ width: `${fill}%` }}
                        >
                            <Star
                                size={size}
                                className="fill-yellow-400 text-yellow-400"
                            />
                        </div>
                    </div>
                );
            })}
        </div>
    );
}